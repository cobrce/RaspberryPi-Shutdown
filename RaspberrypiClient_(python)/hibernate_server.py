# on raspberry
import socket
from time import sleep

computer_name = "CYBERSPACE.local"
port = 1992

addr = (computer_name,port)

def hibernate_server(inifinite):
    while True:
        try:
            s = socket.create_connection(addr,timeout=1)
            s.send(b"hibernate")
            return
        except Exception as e:
            print(e)
        if not inifinite:
            return
if __name__ == "__main__":
    hibernate_server(True)
