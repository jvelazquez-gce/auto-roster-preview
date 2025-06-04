using System.Collections.Generic;

namespace Domain.Constants
{
    public static class SqlErrorCodes
    {
        static public List<int> TRANSIANT_SQL_ERROR_CODE_LIST = new List<int>(
            new int[]
                {
                    -2,     // Timeout
                    20,     // DBNETLIB Error Code: 20
                            // The instance of SQL Server you attempted to connect to does not support encryption.
                    64,     // SQL Error Code: 64
                            // A connection was successfully established with the server, but then an error occurred during the login process. 
                            // (provider: TCP Provider, error: 0 - The specified network name is no longer available.) 
                    233,    // SQL Error Code: 233
                            // The client was unable to establish a connection because of an error during connection initialization process before login. 
                            // Possible causes include the following: the client tried to connect to an unsupported version of SQL Server; the server was too busy 
                            // to accept new connections; or there was a resource limitation (insufficient memory or maximum allowed connections) on the server. 
                            // (provider: TCP Provider, error: 0 - An existing connection was forcibly closed by the remote host.)
                    1205,   // Deadlock
                    10053,  // SQL Error Code: 10053
                            // A transport-level error has occurred when receiving results from the server.
                            // An established connection was aborted by the software in your host machine.
                    10054,  // SQL Error Code: 10054
                            // A transport-level error has occurred when sending the request to the server. 
                            // (provider: TCP Provider, error: 0 - An existing connection was forcibly closed by the remote host.)
                    10060,  // SQL Error Code: 10060
                            // A network-related or instance-specific error occurred while establishing a connection to SQL Server. 
                            // The server was not found or was not accessible. Verify that the instance name is correct and that SQL Server 
                            // is configured to allow remote connections. (provider: TCP Provider, error: 0 - A connection attempt failed 
                            // because the connected party did not properly respond after a period of time, or established connection failed 
                            // because connected host has failed to respond.)"}
                    10928,  // SQL Error Code: 10928
                            // Resource ID: %d. The %s limit for the database is %d and has been reached.
                    10929,  // SQL Error Code: 10929
                            // Resource ID: %d. The %s minimum guarantee is %d, maximum limit is %d and the current usage for the database is %d. 
                            // However, the server is currently too busy to support requests greater than %d for this database.
                    40143,  // SQL Error Code: 40143
                            // The service has encountered an error processing your request. Please try again.
                    40197,  // SQL Error Code: 40197
                            // The service has encountered an error processing your request. Please try again.
                    40501,  // SQL Error Code: 40501
                            // The service is currently busy. Retry the request after 10 seconds. Code: (reason code to be decoded).
                    40540,  // SQL Error Code: 40540
                            // The service has encountered an error processing your request. Please try again.
                    40613,  // SQL Error Code: 40613
                            // Database XXXX on server YYYY is not currently available. Please retry the connection later. If the problem persists, contact customer 
                            // support, and provide them the session tracing ID of ZZZZZ.
                });
    }
}
