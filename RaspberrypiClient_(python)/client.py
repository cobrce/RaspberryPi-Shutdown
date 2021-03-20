# should be run in raspberry with sudo 

import os
import sys
import RPi.GPIO as GPIO
import time

from hibernate_server import hibernate_server

def set_led_item(index,item,value):
    with open("/sys/class/leds/led%d/%s" % (index,item),"w") as f:
        f.write(value)


# turn off red LED
set_led_item(1,"trigger","none")
set_led_item(1,"brightness","0")

# turn on green LED
set_led_item(0,"trigger","none")
set_led_item(0,"brightness","0")
time.sleep(0.5) # I had to use this trick to make the green led stay on
set_led_item(0,"brightness","1")

# should run the script with "shutdown_button" to handle shutdown button/relay
# code inspired from this tutorial https://www.quartoknows.com/page/raspberry-pi-shutdown-button
if len(sys.argv) == 2 and sys.argv[1] == "shutdown_button":
    print("shutdown button enabled")

    # Use the Broadcom SOC Pin numbers
    GPIO.setmode(GPIO.BCM)

    # Setup the pin with internal pullups enabled and pin in reading mode.
    GPIO.setup(21, GPIO.IN, pull_up_down=GPIO.PUD_UP)    

    # Our function on what to do when the button is pressed
    def Shutdown(channel):
        print("shutdown pressed")
        brightness = 0
        now = time.time() # get current time
        brightness_timer = now
        while time.time() - now < 5: # if during 5 seconds the channel is high
            if GPIO.input(channel): # abort shutdown
                set_led_item(0,"brightness","0")
                time.sleep(0.2)
                set_led_item(0,"brightness","1")
                print("shutdown aborted")
                return

            if time.time() - brightness_timer >= 0.1: # blink green led to display response to button
                print(brightness)
                set_led_item(0,"brightness","%d" % brightness)
                brightness_timer = time.time()
                brightness = 1 - brightness

        print("Shutting Down")

        set_led_item(0,"trigger","none")
        set_led_item(0,"brightness","0") # turn off green led to display that it's going to shutdown
        hibernate_server(False) # send 'hibernate' command to PC
        os.system("sudo shutdown -h now")    
        exit()

    # Add our function to execute when the button pressed event happens
    GPIO.add_event_detect(21, GPIO.FALLING, callback=Shutdown, bouncetime=100)    
    # Now wait!
    while 1:
        time.sleep(1)
