# LadderApp


## A brief history and context

This project was finalized at the end of 2010 as my undergraduate thesis in Information Systems. It reflects some of my areas of interest (development, object-oriented programming, industrial automation, microcontrollers).

At the time, I would have liked to have developed this project in Java Web, but the rush of life, work, and college did not allow me to believe it was a viable work.

Almost ten years have passed since I finished this project. So, last year, I put it on GitHub but, I still had many ideas for it and also had done nothing about these ideas yet.


## The project LadderApp

![LadderApp screen with an open program](images/ladderapp-running.gif)


The project LadderApp allows you to develop a program in ladder language (standard IEC 61131-3), simulate a PLC working, send the executable to a microcontroller, read a previously uploaded file from a microcontroller, and "remount" it in ladder language again.

Today "base" microcontroller code inside the project is designed only for [MSP430](http://www.ti.com/microcontrollers/msp430-ultra-low-power-mcus/overview.html) microcontroller from Texas Instruments.

The application has four main functions:

- Develop a ladder program in the GUI;
- Simulate a PLC/microcontroller work inside it without any extra hardware;
- Write (download) that program to the microcontroller to be executed;
- Read (upload) a previously written ladder program from a microcontroller and "remount" the ladder at the GUI.


### Writing to a microcontroller

To this work, the application will perform the conversion of a ladder program developed at GUI into a C language program to the final microcontroller, allowing this program to be compiled and sent to the microcontroller. 

In the application, with an opened ladder program at the GUI, ready to work, when you click on the menu "Microcontroller/Communication/Download Program", the application will convert that ladder program to a C language program in the background, with the whole base code for the PLC work linked with ladder logic developed in the application. After that, it will compile the generated C program (using [MSPGCC](https://www.ti.com/application/MSP430-GCC-OPENSOURCE) - you need to install first), then it will send executable (the compilation result) to the microcontroller connected on USB (using MSPJTAG USB application).


### Reading from a microcontroller

When you write (download) a ladder program to the microcontroller, an option could be enabled (default option) to save the ladder logic inside the executable written to the microcontroller. Then, when you read a program from a microcontroller in which that option previously enabled, the application could "remount" the ladder at the GUI from that executable program read.


### Simulating a microcontroller/plc work

The application allows the user to simulate the execution of the ladder code directly on the GUI interface and interact with the execution toggle bit and toggle bit pulse options.


### Future ideas or Roadmap

I have many ideas for this project, and some of them are:

- English translation (on going);
- Include unit testing;
- Big code refactoring;
- Extend the "base" microcontroller code to others platforms [Arduino](https://www.arduino.cc/), PIC, STM32, ...;
- Allow to type the program textually and create a "compiler front-end" that checks the syntax and semantics of the ladder language(using [ANTLR](https://www.antlr.org/));
- Migrate to SaaS;
- Internationalization

Enjoy.

Tales Wallace Souza