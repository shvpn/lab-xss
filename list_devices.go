package main

import (
	"fmt"

	"github.com/google/gopacket/pcap"
)

func main() {
	devices, err := pcap.FindAllDevs()
	if err != nil {
		fmt.Println("Error finding devices:", err)
		return
	}

	fmt.Println("Available network devices:")
	fmt.Println("=========================")
	for i, dev := range devices {
		fmt.Printf("\n[%d] Device: %s\n", i+1, dev.Name)
		fmt.Printf("    Description: %s\n", dev.Description)
		fmt.Printf("    Addresses:\n")
		for _, addr := range dev.Addresses {
			fmt.Printf("      - IP: %s\n", addr.IP)
		}
	}
}
