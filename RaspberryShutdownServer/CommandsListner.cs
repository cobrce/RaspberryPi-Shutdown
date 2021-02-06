using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

class CommandsLisnter
{
	readonly Socket socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
	readonly Dictionary<string, Action<CommandsLisnter, string>> commands = new Dictionary<string, Action<CommandsLisnter, string>>();

	public CommandsLisnter(int port)
	{
		socket.Bind(new IPEndPoint(0, port));
		socket.Listen(0);
	}

	public void AddCommand(string command, Action<CommandsLisnter, string> action)
	{
		commands[command] = action;
	}

	public void Loop()
	{
		while (true)
		{
			try
			{
				var client = socket.Accept();
				for (int i = 0; i < 10; i++)
				{
					if (client.Available > 0)
					{
						byte[] data = new byte[client.Available];
						client.Receive(data);
						string command = Encoding.Default.GetString(data);
						if (commands.Keys.Contains(command))
							commands[command](this, command);
						break;
					}
					System.Threading.Thread.Sleep(500);
				}
				client.Close();
			}
			catch { }
		}
	}
}