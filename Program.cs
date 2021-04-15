using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using OpenHardwareMonitor;
using OpenHardwareMonitor.Hardware;

namespace ArduinoHWMonitor {
    class Program {
        // Member data
        Computer thisComputer;                              // To store the reference to this computer
        
        // Temperature stats
        String cpuTemp;                                     // To store CPU temperature
        String gpuTemp;                                     // To store GPU temperature

        // Usage stats
        String cpuUsage;                                    // To store CPU usage
        String gpuUsage;                                    // To store GPU usage

        // Member method
        // Constructor
        Program() {
            // Creating new connection to get GPU and CPU data
            thisComputer = new Computer() { CPUEnabled = true, GPUEnabled = true };

            // Opening connection
            thisComputer.Open();
        }

        // Method to get CPU and GPU stats
        private String getStats() {
            // Getting each hardware item
            foreach(var hwItem in thisComputer.Hardware) {
                // Checking if hardware is CPU
                if(hwItem.HardwareType == HardwareType.CPU) {
                    // Updating to get newest stats
                    hwItem.Update();
                    // Getting each stat
                    foreach(var sensor in hwItem.Sensors) {
                        // For temp
                        if(sensor.SensorType == SensorType.Temperature) {
                            cpuTemp = sensor.Value.Value.ToString();
                        }
                        // For usage
                        else if(sensor.SensorType == SensorType.Load) {
                            cpuUsage = sensor.Value.Value.ToString();
                        }
                    }
                }
                // Checking if hardware is GPU
                else if (hwItem.HardwareType == HardwareType.GpuNvidia) {
                    // Updating to get newest stats
                    hwItem.Update();
                    // Getting each stat
                    foreach (var sensor in hwItem.Sensors) {
                        // For temp
                        if (sensor.SensorType == SensorType.Temperature) {
                            gpuTemp = sensor.Value.Value.ToString();
                        }
                        // For usage
                        else if (sensor.SensorType == SensorType.Load) {
                            gpuUsage = sensor.Value.Value.ToString();
                        }
                    }
                }
            }

            // Returning data
            return (cpuTemp + "\t\t" + cpuUsage + "\n" + gpuTemp + "\t\t" + gpuUsage);
        }

        // Main method
        public static void Main(String[] args) {
            Program ob = new Program();
            while(true) {
                Console.WriteLine(ob.getStats());
                Thread.Sleep(1000);
            }
        }
    }
}
