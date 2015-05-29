======================================================================================================================================================
-                                   MAREA2: Middleware Architecture for Remote Embedded Applications 2                                               -        
======================================================================================================================================================

Marea middleware proposes a modular architecture based on services. This middleware combines both client-server and multi-point message communications 
and it is specially suited to communicate low-cost components interconnected by local networks.

Middleware-based software systems consist of a network of cooperating components which implement the logic of the system and an integrating middleware        layer that abstracts the execution environment and implements common functionalities and communication channels. In this view, the different components 
are semantic units that behave as producers of data and as consumers of data coming from other components. The localization of the other components is 
not important because the middleware manages their discovery. The middleware also handles all the  transfer chores: message addressing, data marshaling
and demarshalling delivery, flow control, retries, etc.

Most of the present middleware is focused on client-server relationships between components. While this approach is applicable for most systems, it does      not take benefit of the multicast capabilities of local networks. Publish-Subscribe communications are better suited for this sort of systems. Marea        combines both client-server and multi-point message communications and it is specially suited to communicate low-cost components interconnected by local      networks.
 
--------------------------------------
-       Solution structure           -
--------------------------------------                                   
- Marea      
- Marea.Console
- Marea.Gen                         
- Marea.Interface 
- Marea.UnitTests
- Marea.UnitTests.Entities
- Marea.NugetServer
- /Examples/Marea.Examples.IDU
- /Examples/Marea.Examples.SDU
- /Libs
- /Files

- Marea.PerfomanceTests
- Marea.Services                      
                                  
MAREA 2 Contributors:
	Juan L�pez  (juan.lopez-rubio@upc.edu)
	Santi P�rez (santiago.perez-fernandez@estudiant.upc.edu)
