# ARP Spoofing Scripts for Windows

## Overview
Converted the Linux-based ARP spoofing script to Windows-compatible versions.

## Available Versions

### 1. Python + Scapy (Recommended)
**File**: `arp_spoof_windows.py`

**Installation**:
```powershell
# Install Python 3 (if not already installed)
# Download from: https://www.python.org/downloads/

# Install Npcap (required for Scapy on Windows)
# Download from: https://npcap.com/#download

# Install Scapy
pip install scapy
```

**Usage**:
```powershell
# Run as Administrator
python arp_spoof_windows.py <gateway_ip> <subnet>

# Example:
python arp_spoof_windows.py 192.168.1.1 192.168.1.0/24
```

**Features**:
- Full ARP spoofing functionality
- Scans for active hosts
- Restores ARP tables on exit
- Works on Windows with Npcap

---

### 2. PowerShell (Limited)
**File**: `arp_spoof_windows.ps1`

**Usage**:
```powershell
# Run as Administrator
.\arp_spoof_windows.ps1 -GatewayIP 192.168.1.1 -Subnet 192.168.1.0/24
```

**Features**:
- Scans for active hosts
- Shows network information
- Limited packet crafting (PowerShell limitation)
- Mainly for reconnaissance

**Note**: PowerShell doesn't have native raw packet crafting. Use Python version for actual spoofing.

---

### 3. C# with SharpPcap (Advanced)
**File**: `arp_spoof_csharp.cs`

**Installation**:
```powershell
# Option 1: Visual Studio
# - Create new Console App
# - Install NuGet packages: SharpPcap, PacketDotNet
# - Copy code and build

# Option 2: Command line with csc
# - Install NuGet packages manually
# - Compile: csc /reference:SharpPcap.dll,PacketDotNet.dll arp_spoof_csharp.cs
```

**Usage**:
```powershell
# Run as Administrator
.\arp_spoof_csharp.exe 192.168.1.1 192.168.1.100
```

**Features**:
- Native Windows performance
- Uses SharpPcap library
- Full packet crafting capabilities
- Compiled executable

---

## Prerequisites

### All Versions
- Windows 10/11
- Administrator privileges
- Npcap driver installed

### Npcap Installation
1. Download from: https://npcap.com/#download
2. Install with "WinPcap API-compatible Mode" enabled
3. Restart computer

---

## Important Notes

### Legal Warning
⚠️ **ARP spoofing is illegal without authorization**
- Only use on networks you own or have explicit permission to test
- Educational purposes only
- Can disrupt network operations

### Technical Notes
- Windows Defender may flag these as malicious (they are network tools)
- Some antivirus software may block execution
- Requires administrator privileges for raw packet access
- Works best on local network segments

### Comparison

| Feature | Python + Scapy | PowerShell | C# + SharpPcap |
|---------|---------------|------------|----------------|
| Ease of Use | ⭐⭐⭐⭐⭐ | ⭐⭐⭐⭐ | ⭐⭐⭐ |
| Functionality | Full | Limited | Full |
| Performance | Good | Poor | Excellent |
| Setup Complexity | Low | None | High |
| **Recommended** | ✅ Yes | ❌ No | ⚠️ Advanced |

---

## Troubleshooting

### "No such device" or "Device not found"
- Install Npcap driver
- Restart computer
- Run as Administrator

### "Permission denied"
- Right-click → Run as Administrator
- Check User Account Control (UAC) settings

### Scapy import errors
```powershell
pip install --upgrade scapy
pip install pywin32
```

### No hosts found
- Check subnet notation (e.g., 192.168.1.0/24)
- Verify you're on the same network
- Check firewall settings

---

## Alternatives

If these don't work, consider:
- **Ettercap for Windows**: GUI-based MITM tool
- **Cain & Abel**: Classic Windows network tool
- **WSL2 + Kali**: Run Linux tools on Windows

---

## Original File
The original file `ddos_learn.go` was actually a Python script with incorrect extension. It has been properly converted for Windows compatibility.
