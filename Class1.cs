using System.Net.NetworkInformation;
using System.Net;
using System;

public class NetworkHelper
{
    public static bool IsNetworkAvailable(out string errorMessage)
    {
        errorMessage = null;

        try
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
            {
                errorMessage = "No network connection available.";
                return false;
            }

            // Check internet connectivity by pinging a reliable server
            const string reliableServer = "8.8.8.8"; // Google DNS server
            var ping = new Ping();
            var reply = ping.Send(reliableServer);

            if (reply == null || reply.Status != IPStatus.Success)
            {
                errorMessage = "Could not ping a reliable server. Check your internet connection.";
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            errorMessage = "An error occurred while checking network connectivity: " + ex.Message;
            return false;
        }
    }
}