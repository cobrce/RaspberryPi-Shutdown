using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RaspberryShutdownServer
{
	public partial class Form1 : Form
	{
		CommandsLisnter CommandsLisnter = new CommandsLisnter(1992);
		private bool handlingHibernateCommand = false;
		private int counter;

		public Form1()
		{
			InitializeComponent();
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			CancelButton = btnCancel;
			AcceptButton = btnNow;
			CommandsLisnter.AddCommand("hibernate", HibernateCommandHandler);
			backgroundWorker1.DoWork += BackgroundWorker1_DoWork;
			backgroundWorker1.RunWorkerAsync();
		}

		void SetLabelText()
		{
			SetLabelText($"Hibernating in {counter} seconds");
		}

		void SetLabelText(string text)
		{
			label1.Invoke(new Action(() =>
			{
				label1.Text = text;
			}));
		}

		private void HibernateCommandHandler(CommandsLisnter arg1, string arg2)
		{
			Invoke(new Action(() => { btnNow.Enabled = true; }));
			if (!handlingHibernateCommand) // just to make sure to process only one time
			{
				counter = 5;
				SetLabelText();
				handlingHibernateCommand = true;
				Invoke(new Action(Show));
				Invoke(new Action(timer1.Start));
			}
		}

		private void BackgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
		{
			CommandsLisnter.Loop();
		}

		private void Form1_Shown(object sender, EventArgs e)
		{
			Hide();
			notifyIcon1.Icon = this.Icon;
			notifyIcon1.Visible = true;
		}

		private void notifyIcon1_DoubleClick(object sender, EventArgs e)
		{
			Environment.Exit(0);
		}

		private void timer1_Tick(object sender, EventArgs e)
		{
			counter--;
			SetLabelText();
			if (counter == 0)
			{
				Hibernate(true);
			}
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			Hibernate(false);
		}

		private void btnNow_Click(object sender, EventArgs e)
		{
			Hibernate(true);
		}

		private void Hibernate(bool hibernate)
		{
			Invoke(new Action(timer1.Stop));
			if (hibernate)
			{
				Application.SetSuspendState(PowerState.Hibernate, true, true);
				handlingHibernateCommand = false;
				Invoke(new Action(Hide));
			}
			else
			{
				SetLabelText("Aborted");
				Invoke(new Action(timer2.Start));
			}
		}

		private void Form1_FormClosing(object sender, FormClosingEventArgs e)
		{
			e.Cancel = true;
			Hibernate(false);
		}

		private void timer2_Tick(object sender, EventArgs e)
		{
			handlingHibernateCommand = false;
			Invoke(new Action(Hide));
			Invoke(new Action(timer2.Stop));
		}
	}
}
