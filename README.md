GoFreeWebSocketTest
===================

[![Join the chat at https://gitter.im/parsley72/GoFreeWebSocketTest](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/parsley72/GoFreeWebSocketTest?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

Navico GoFree WebSocket test

Simple test program written in C# that works with Visual Studio 2010 or 2012 Express. It uses the Navico service discovery to find a device that offers this service ("navico-nav-ws"), connects to it and runs all the possible websocket commands.

The Websocket protocol is specified here:
[http://www.simrad-yachting.com/Global/Simrad-Yachting/Products/GoFree/GoFree%20Tier2%20Toolkit%20%28view%20only%29.pdf](http://www.simrad-yachting.com/Global/Simrad-Yachting/Products/GoFree/GoFree%20Tier2%20Toolkit%20%28view%20only%29.pdf)

Command line options:

-v Verbose

-m Model (e.g. NSS)

-i IP address

-p Port
