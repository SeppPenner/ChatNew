@echo off

cls

REM ------ To compile ChatServer-----------------------
echo Compiling ChatServer ...
echo csc /out:ChatServer.exe /r:ShapeControl.dll  *.cs /main:Chat.ChatServer

csc /out:ChatServer.exe *.cs /r:ShapeControl.dll /main:Chat.ChatServer
REM ---------------------------------------------------

REM ------ To compile ChatClient ----------------------
echo Compiling ChatClient ...
echo csc /out:ChatClient.exe /r:ShapeControl.dll *.cs /main:Chat.ChatClient
csc /out:ChatClient.exe /r:ShapeControl.dll *.cs /main:Chat.ChatClient
REM ---------------------------------------------------