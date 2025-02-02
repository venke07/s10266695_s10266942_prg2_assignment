//==========================================================
// Student Number : S10266695 , S10266942
// Student Name :  T Venkatesh
// Partner Name : Pugazhenthi Dharundev
//==========================================================
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace s10266695_s10266942_prg2_assignment
{
    internal class Program
    {
        
        static Dictionary<string, string> flightSpecialRequests = new Dictionary<string, string>();
        static Dictionary<string, BoardingGate> flightGateAssignments = new Dictionary<string, BoardingGate>();

        static void Main(string[] args)
        {
            
            string flightsFilePath = "flights.csv";
            string airlinesFilePath = "airlines.csv";
            string boardingGatesFilePath = "boardinggates.csv";

            // Load the data from CSV files
            Dictionary<string, Flight> flights = LoadFlights(flightsFilePath);
            Dictionary<string, Airline> airlines = LoadAirlines(airlinesFilePath, flights);
            Dictionary<string, BoardingGate> boardingGates = LoadBoardingGates(boardingGatesFilePath);
            Dictionary<string, double> gateFees = new Dictionary<string, double>
                {
                       { "CFFT", 100.0 },
                       { "DDJB", 150.0 },
                       { "LWTT", 200.0 },
                       { "NORM", 50.0 }
                };


            // Main menu loop
            while (true)
            {
                Console.WriteLine("\n=============================================");
                Console.WriteLine("Welcome to Changi Airport Terminal 5");
                Console.WriteLine("=============================================");
                Console.WriteLine("1. List All Flights");
                Console.WriteLine("2. List Boarding Gates");
                Console.WriteLine("3. Assign a Boarding Gate to a Flight");
                Console.WriteLine("4. Create Flight");
                Console.WriteLine("5. Display Airline Flights");
                Console.WriteLine("6. Modify Flight Details");
                Console.WriteLine("7. Display Flight Schedule");
                Console.WriteLine("8. Process Unassigned Flights");
                Console.WriteLine("9. Display Total Fee Per Airline");
                Console.WriteLine("10. Check Schedule Conflicts");
                Console.WriteLine("11. Search Flights by Time Window");
                Console.WriteLine("0. Exit");
                Console.Write("Please select your option: ");

                string? choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        ListFlights(airlines);
                        break;
                    case "2":
                        ListBoardingGates(boardingGates);
                        break;
                    case "3":
                        AssignBoardingGate(flights, boardingGates, flightSpecialRequests);
                        break;
                    case "4":
                        CreateNewFlight(flights, flightsFilePath);
                        break;
                    case "5":
                        DisplayFlightDetailsByAirline(airlines, flights);
                        break;
                    case "6":
                        ModifyFlightDetails(airlines, flights);
                        break;
                    case "7":
                        DisplayScheduledFlights(flights, airlines);
                        break;
                    case "8":
                        ProcessUnassignedFlights(flights, boardingGates);
                        break;
                    case "9":
                        DisplayTotalFeesPerAirline(airlines, flights, boardingGates, gateFees);
                        break;
                    case "10":
                        DetectScheduleConflicts(flights, boardingGates, flightGateAssignments);
                        break;
                    case "11":
                        SearchFlightsInTimeWindow(flights);
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }
        }

        // --- CSV Loading Methods ---
        static Dictionary<string, Flight> LoadFlights(string filePath)
        {
            var flights = new Dictionary<string, Flight>();
            if (File.Exists(filePath))
            {
                var lines = File.ReadAllLines(filePath);
                // Assume first line is a header
                for (int i = 1; i < lines.Length; i++)
                {
                    var fields = lines[i].Split(',');
                    if (fields.Length >= 5 && DateTime.TryParse(fields[3].Trim(), out DateTime expectedTime))
                    {
                        string flightNumber = fields[0].Trim();
                        string origin = fields[1].Trim();
                        string destination = fields[2].Trim();
                        string status = fields[4].Trim();
                        string specialReq = fields.Length >= 6 ? fields[5].Trim().ToUpper() : "";
                        double requestFee = 0;

                        // Validate special request code
                        if (!string.IsNullOrEmpty(specialReq) && specialReq != "CFFT" &&
                            specialReq != "DDJB" && specialReq != "LWTT")
                        {
                            Console.WriteLine($"Warning: Invalid special request code '{specialReq}' for flight {flightNumber}. Setting to NORM.");
                            specialReq = "";
                        }

                        // Create appropriate flight type based on special request
                        Flight flight;
                        switch (specialReq)
                        {
                            case "CFFT":
                                flight = new CFFTFlight(flightNumber, origin, destination, expectedTime, status, requestFee);
                                flightSpecialRequests[flightNumber] = "CFFT";
                                break;
                            case "DDJB":
                                flight = new DDJBFlight(flightNumber, origin, destination, expectedTime, status, requestFee);
                                flightSpecialRequests[flightNumber] = "DDJB";
                                break;
                            case "LWTT":
                                flight = new LWTTFlight(flightNumber, origin, destination, expectedTime, status, requestFee);
                                flightSpecialRequests[flightNumber] = "LWTT";
                                break;
                            default:
                                flight = new NORMFlight(flightNumber, origin, destination, expectedTime, status);
                                break;
                        }

                        flights[flightNumber] = flight;
                    }
                }
                Console.WriteLine("Flights loaded successfully.");
            }
            else
            {
                Console.WriteLine($"File not found: {filePath}");
            }
            return flights;
        }

        // CSV for airlines:
        static Dictionary<string, Airline> LoadAirlines(string filePath, Dictionary<string, Flight> flights)
        {
            var airlines = new Dictionary<string, Airline>();
            if (File.Exists(filePath))
            {
                var lines = File.ReadAllLines(filePath);
                
                for (int i = 1; i < lines.Length; i++)
                {
                    var fields = lines[i].Split(',');
                    if (fields.Length >= 2)
                    {
                       
                        Airline airline = new Airline(fields[0].Trim(), fields[1].Trim(), flights);
                        airlines[airline.Code] = airline;

                    }
                }
                Console.WriteLine("Airlines loaded successfully.");
            }
            else
            {
                Console.WriteLine($"File not found: {filePath}");
            }
            return airlines;
        }

        //CSV for boarding gates:
        static Dictionary<string, BoardingGate> LoadBoardingGates(string filePath)
        {
            var gates = new Dictionary<string, BoardingGate>();
            if (File.Exists(filePath))
            {
                var lines = File.ReadAllLines(filePath);
                
                for (int i = 1; i < lines.Length; i++)
                {
                    var fields = lines[i].Split(',');
                    if (fields.Length >= 4 &&
                        bool.TryParse(fields[1].Trim(), out bool supportsDDJB) &&
                        bool.TryParse(fields[2].Trim(), out bool supportsCFFT) &&
                        bool.TryParse(fields[3].Trim(), out bool supportsLWTT))
                    {
                        BoardingGate gate = new BoardingGate(fields[0].Trim(), supportsDDJB, supportsCFFT, supportsLWTT, null);
                        gates[gate.GateName] = gate;
                    }
                }
            }
            return gates;
        }

        // --- Menu Option Methods ---
        static void ListFlights(Dictionary<string, Airline> airlines)
        {
            Console.WriteLine("===============================================================");
            Console.WriteLine("       List of Boarding Gates for Changi Airport Terminal 5");
            Console.WriteLine("===============================================================");

            
            Console.WriteLine(String.Format("{0,-20} {1,-30} {2,-20} {3,-20} {4,-35}",
                "Flight Number", "Airline Name", "Origin", "Destination", "Expected Departure/Arrival Time"));
            Console.WriteLine(new string('-', 100)); 

            
            foreach (var airline in airlines.Values)
            {
                foreach (var flight in airline.Flights.Values)
                {
                    Console.WriteLine(String.Format("{0,-20} {1,-30} {2,-20} {3,-20} {4,-35}",
                        flight.FlightNumber,
                        airline.Name,
                        flight.Origin,
                        flight.Destination,
                        flight.ExpectedTime.ToString("dd/MM/yyyy HH:mm")));
                }
            }
        }

        // List boarding gates
        static void ListBoardingGates(Dictionary<string, BoardingGate> gates)
        {
            Console.WriteLine("=".PadRight(50, '='));
            Console.WriteLine("\nList of Boarding Gates for Changi Airport Terminal 5\n");
            Console.WriteLine("=".PadRight(50, '='));
            Console.WriteLine("\nGate Name  DDJB    CFFT    LWTT");

            foreach (var gate in gates.Values.OrderBy(g => g.GateName[0]) // First sort by the letter
                                           .ThenBy(g =>                    // Then sort by the number
                                           {
                                               string numberPart = g.GateName.Substring(1);
                                               if (int.TryParse(numberPart, out int number))
                                                   return number;
                                               return 0;
                                           }))
            {
                Console.WriteLine($"{gate.GateName,-10} {gate.SupportsDDJB,-7} {gate.SupportsCFFT,-7} {gate.SupportsLWTT,-6}");
            }
        }

        // Assign a boarding gate to a flight.
        static void AssignBoardingGate(Dictionary<string, Flight> flights, Dictionary<string, BoardingGate> gates, Dictionary<string, string> specialRequests)
        {
            Console.WriteLine("=============================================");
            Console.WriteLine("Assign a Boarding Gate to a Flight");
            Console.WriteLine("=============================================");

            Console.Write("Enter Flight Number: ");
            string? flightNumber = Console.ReadLine();

            if (string.IsNullOrEmpty(flightNumber) || !flights.TryGetValue(flightNumber, out Flight flight))
            {
                Console.WriteLine("Flight not found.");
                return;
            }

            Console.Write("Enter Boarding Gate Name: ");
            string? gateName = Console.ReadLine();

            if (string.IsNullOrEmpty(gateName) || !gates.TryGetValue(gateName, out BoardingGate gate))
            {
                Console.WriteLine("Gate not found.");
                return;
            }

            string specialRequest = specialRequests.ContainsKey(flightNumber) ? specialRequests[flightNumber] : "None";

            Console.WriteLine($"\nFlight Number: {flight.FlightNumber}");
            Console.WriteLine($"Origin: {flight.Origin}");
            Console.WriteLine($"Destination: {flight.Destination}");
            Console.WriteLine($"Expected Time: {flight.ExpectedTime:dd/MM/yyyy HH:mm tt}");
            Console.WriteLine($"Special Request Code: {specialRequest}");
            Console.WriteLine($"Boarding Gate Name: {gate.GateName}");
            Console.WriteLine($"Supports DDJB: {gate.SupportsDDJB}");
            Console.WriteLine($"Supports CFFT: {gate.SupportsCFFT}");
            Console.WriteLine($"Supports LWTT: {gate.SupportsLWTT}");
            Console.WriteLine();

            Console.Write("Would you like to update the status of the flight? (Y/N): ");
            string? updateResponse = Console.ReadLine();
            if (!string.IsNullOrEmpty(updateResponse) && updateResponse.Trim().ToUpper() == "Y")
            {
                Console.WriteLine("1. Delayed");
                Console.WriteLine("2. Boarding");
                Console.WriteLine("3. On Time");
                Console.Write("Please select the new status of the flight: ");
                string? statusOption = Console.ReadLine();

                switch (statusOption)
                {
                    case "1":
                        flight.Status = "Delayed";
                        break;
                    case "2":
                        flight.Status = "Boarding";
                        break;
                    case "3":
                        flight.Status = "On Time";
                        break;
                    default:
                        Console.WriteLine("Invalid option. Flight status remains unchanged.");
                        break;
                }
            }

            flightGateAssignments[flight.FlightNumber] = gate;
            gate.Flight = flight;

            Console.WriteLine($"\nFlight {flight.FlightNumber} has been assigned to Boarding Gate {gate.GateName}!");
        }
        static void CreateNewFlight(Dictionary<string, Flight> flights, string filePath)
        {
            while (true) // Loop 
            {
                Console.Write("Enter Flight Number: ");
                string? flightNumber = Console.ReadLine();
                if (string.IsNullOrEmpty(flightNumber))
                {
                    Console.WriteLine("Flight number cannot be empty.");
                    return;
                }

                Console.Write("Enter Origin: ");
                string? origin = Console.ReadLine();
                Console.Write("Enter Destination: ");
                string? destination = Console.ReadLine();

                DateTime expectedTime;
                while (true) // Loop 
                {
                    Console.Write("Enter Expected Departure/Arrival Time (dd/MM/yyyy HH:mm): ");
                    string? inputDate = Console.ReadLine();

                    if (DateTime.TryParseExact(inputDate, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out expectedTime))
                    {
                        break; // Exit loop 
                    }
                    else
                    {
                        Console.WriteLine("Invalid date/time format. Please use dd/MM/yyyy HH:mm.");
                    }
                }

                Console.Write("Enter Special Request Code (CFFT/DDJB/LWTT/None): ");
                string specialRequest = Console.ReadLine()?.ToUpper() ?? "NONE";

                Flight flight;
                double requestFee = 0; // Set request fee based on type

                switch (specialRequest)
                {
                    case "CFFT":
                        flight = new CFFTFlight(flightNumber, origin ?? "", destination ?? "", expectedTime, "On Time", requestFee);
                        break;
                    case "DDJB":
                        flight = new DDJBFlight(flightNumber, origin ?? "", destination ?? "", expectedTime, "On Time", requestFee);
                        break;
                    case "LWTT":
                        flight = new LWTTFlight(flightNumber, origin ?? "", destination ?? "", expectedTime, "On Time", requestFee);
                        break;
                    default:
                        flight = new NORMFlight(flightNumber, origin ?? "", destination ?? "", expectedTime, "On Time");
                        specialRequest = "None"; 
                        break;
                }

                flights[flightNumber] = flight;

                // Append to CSV file
                string line = $"{flight.FlightNumber},{flight.Origin},{flight.Destination},{flight.ExpectedTime:dd/MM/yyyy HH:mm},{flight.Status},{specialRequest}\n";
                File.AppendAllText(filePath, line);

                Console.WriteLine($"Flight {flight.FlightNumber} has been added!");

                Console.Write("Would you like to add another flight? (Y/N): ");
                string? response = Console.ReadLine()?.Trim().ToUpper();
                if (response != "Y")
                {
                    break; // Exit loop 
                }
            }
        }
        // Display flights ordered by expected time.
        static void DisplayScheduledFlights(Dictionary<string, Flight> flights, Dictionary<string, Airline> airlines)
        {
            Console.WriteLine("=====================================================================");
            Console.WriteLine("       Flight Schedule for Changi Airport Terminal 5");
            Console.WriteLine("=====================================================================");
            Console.WriteLine(String.Format("{0,-20} {1,-30} {2,-20} {3,-20} {4,-25} {5,-20} {6,-20}",
                "Flight Number", "Airline Name", "Origin", "Destination", "Expected Departure/Arrival Time", "Status", "Boarding Gate"));
            Console.WriteLine(new string('-', 120));

            foreach (var flight in flights.Values.OrderBy(f => f.ExpectedTime))
            {
                // Find the airline that owns this flight
                string airlineName = "Unknown Airline";
                foreach (var airline in airlines.Values)
                {
                    if (flight.FlightNumber.StartsWith(airline.Code)) // Assuming flight numbers start with airline code
                    {
                        airlineName = airline.Name;
                        break;
                    }
                }

                string assignedGate = flightGateAssignments.ContainsKey(flight.FlightNumber)
                    ? flightGateAssignments[flight.FlightNumber].GateName
                    : "Unassigned";

                Console.WriteLine(String.Format("{0,-20} {1,-30} {2,-20} {3,-20} {4,-25} {5,-20} {6,-20}",
                    flight.FlightNumber,
                    airlineName,
                    flight.Origin,
                    flight.Destination,
                    flight.ExpectedTime.ToString("dd/MM/yyyy HH:mm"),
                    flight.Status,
                    assignedGate));
            }
        }
        // Display flights for a given airline.
        static void DisplayFlightDetailsByAirline(Dictionary<string, Airline> airlines, Dictionary<string, Flight> flights)
        {
            // Display available airlines
            Console.WriteLine("\n--- Available Airlines ---");
            foreach (var airline in airlines.Values)
            {
                Console.WriteLine($"Airline Code: {airline.Code} - Name: {airline.Name}");
            }

            Console.Write("\nEnter 2-Letter Airline Code: ");
            string? airlineCode = Console.ReadLine()?.ToUpper();
            if (string.IsNullOrEmpty(airlineCode) || !airlines.ContainsKey(airlineCode))
            {
                Console.WriteLine("Invalid airline code.");
                return;
            }

            Airline selectedAirline = airlines[airlineCode];
            Console.WriteLine($"\n=== List of Flights for {selectedAirline.Name} ===\n");

            Console.WriteLine("\nFlight Number   Airline Name         Origin             Destination       Expected Departure/Arrival Time");

            // Display flights for the selected airline
            var airlineFlights = flights.Values.Where(f => f.FlightNumber.StartsWith(airlineCode));
            if (!airlineFlights.Any())
            {
                Console.WriteLine("No flights found for this airline.");
                return;
            }

            foreach (var flight in airlineFlights.OrderBy(f => f.FlightNumber))
            {
                string code = flight.FlightNumber.Substring(0, 2);
                string airlineName = airlines[code].Name;

                Console.WriteLine($"{flight.FlightNumber,-15} {airlineName,-20} {flight.Origin,-18} {flight.Destination,-17} {flight.ExpectedTime:dd/MM/yyyy HH:mm:}");
            }

            Console.Write("\nEnter Flight Number for detailed information: ");
            string? flightNumber = Console.ReadLine()?.ToUpper();

            if (string.IsNullOrEmpty(flightNumber) || !flights.ContainsKey(flightNumber))
            {
                Console.WriteLine("Invalid flight number.");
                return;
            }

            Flight selectedFlight = flights[flightNumber];

            // Display detailed flight information
            Console.WriteLine("\n=== Detailed Flight Information ===");
            Console.WriteLine($"Flight Number: {selectedFlight.FlightNumber}");
            Console.WriteLine($"Airline Name: {selectedAirline.Name}");
            Console.WriteLine($"Origin: {selectedFlight.Origin}");
            Console.WriteLine($"Destination: {selectedFlight.Destination}");
            Console.WriteLine($"Expected Time: {selectedFlight.ExpectedTime:dd/MM/yyyy HH:mm}");

            // Display special request code if it exists
            if (flightSpecialRequests.ContainsKey(selectedFlight.FlightNumber))
            {
                Console.WriteLine($"Special Request Code: {flightSpecialRequests[selectedFlight.FlightNumber]}");
            }

            // Display boarding gate if assigned
            if (flightGateAssignments.ContainsKey(selectedFlight.FlightNumber))
            {
                Console.WriteLine($"Boarding Gate: {flightGateAssignments[selectedFlight.FlightNumber]}");
            }
            else
            {
                Console.WriteLine("Boarding Gate: Not assigned");
            }
        }


        static void ModifyFlightDetails(Dictionary<string, Airline> airlines, Dictionary<string, Flight> flights)
        {
            // Display available airlines
            Console.WriteLine("\n--- Available Airlines ---");
            foreach (var airline in airlines.Values)
            {
                Console.WriteLine($"Airline Code: {airline.Code} - Name: {airline.Name}");
            }

            Console.Write("\nEnter 2-Letter Airline Code: ");
            string? airlineCode = Console.ReadLine()?.ToUpper();
            if (string.IsNullOrEmpty(airlineCode) || !airlines.ContainsKey(airlineCode))
            {
                Console.WriteLine("Invalid airline code.");
                return;
            }

            Airline selectedAirline = airlines[airlineCode];

            Console.WriteLine($"\n=== List of Flights for {selectedAirline.Name} ===\n");

            Console.WriteLine("\nFlight Number   Airline Name         Origin             Destination       Expected Departure/Arrival Time");

            // Display flights for the selected airline
            var airlineFlights = flights.Values.Where(f => f.FlightNumber.StartsWith(airlineCode));
            if (!airlineFlights.Any())
            {
                Console.WriteLine("No flights found for this airline.");
                return;
            }

            foreach (var flight in airlineFlights.OrderBy(f => f.FlightNumber))
            {
                string code = flight.FlightNumber.Substring(0, 2);
                string airlineName = airlines[code].Name;
                Console.WriteLine($"{flight.FlightNumber,-15} {airlineName,-20} {flight.Origin,-18} {flight.Destination,-17} {flight.ExpectedTime:dd/MM/yyyy HH:mm}");
            }

            // Prompt for flight number 
            Console.Write("\nEnter Flight Number: ");
            string? flightNumber = Console.ReadLine()?.ToUpper();

            if (string.IsNullOrEmpty(flightNumber) || !flights.ContainsKey(flightNumber))
            {
                Console.WriteLine("Invalid flight number.");
                return;
            }

            Flight selectedFlight = flights[flightNumber];

            Console.WriteLine($"\nSelected Flight: {flightNumber}");
            Console.WriteLine("\nChoose an action:");
            Console.WriteLine("[1] Modify flight");
            Console.WriteLine("[2] Delete flight");
            Console.Write("Enter your choice (1 or 2): ");

            string? actionChoice = Console.ReadLine();

            if (actionChoice == "1") 
            {
                Console.WriteLine("\nWhat would you like to modify?");
                Console.WriteLine("1. Modify Basic Information (Origin, Destination, Expected Time)");
                Console.WriteLine("2. Modify Status");
                Console.WriteLine("3. Modify Special Request Code");
                Console.WriteLine("4. Modify Boarding Gate");
                Console.Write("Enter your choice (1-4): ");

                string? modifyChoice = Console.ReadLine();
                switch (modifyChoice)
                {
                    case "1": // Modify Basic Information
                        Console.Write("Enter new Origin: ");
                        string? newOrigin = Console.ReadLine();
                        if (!string.IsNullOrEmpty(newOrigin))
                            selectedFlight.Origin = newOrigin;

                        Console.Write("Enter new Destination: ");
                        string? newDestination = Console.ReadLine();
                        if (!string.IsNullOrEmpty(newDestination))
                            selectedFlight.Destination = newDestination;

                        Console.Write("Enter new Expected Time (dd/MM/yyyy HH:mm): ");
                        string? newTimeStr = Console.ReadLine();
                        if (!string.IsNullOrEmpty(newTimeStr))
                        {
                            if (DateTime.TryParseExact(newTimeStr,
                                                     "dd/MM/yyyy HH:mm",
                                                     CultureInfo.InvariantCulture,
                                                     DateTimeStyles.None,
                                                     out DateTime newTime))
                            {
                                selectedFlight.ExpectedTime = newTime;
                                Console.WriteLine("Time updated successfully!");
                            }
                            else
                            {
                                Console.WriteLine("Invalid date/time format. Please use dd/MM/yyyy HH:mm format.");
                            }
                        }
                        break;

                    case "2": // Modify Status
                        Console.Write("Enter new Status: ");
                        string? newStatus = Console.ReadLine();
                        if (!string.IsNullOrEmpty(newStatus))
                            selectedFlight.Status = newStatus;
                        break;

                    case "3": // Modify Special Request Code
                        Console.Write("Enter new Special Request Code: ");
                        string? newCode = Console.ReadLine();
                        if (string.IsNullOrEmpty(newCode))
                            flightSpecialRequests.Remove(flightNumber);
                        else
                            flightSpecialRequests[flightNumber] = newCode;
                        break;

                    case "4": // Modify Boarding Gate
                        Console.Write("Enter new Boarding Gate Name: ");
                        string? newGate = Console.ReadLine();
                        if (string.IsNullOrEmpty(newGate))
                        {
                            flightGateAssignments.Remove(flightNumber);
                        }
                        else
                        {
                            flightGateAssignments[flightNumber] = new BoardingGate(newGate, false, false, false, selectedFlight);
                        }
                        break;

                    default:
                        Console.WriteLine("Invalid choice.");
                        return;
                }

                Console.WriteLine("Flight updated successfully!");
            }
            else if (actionChoice == "2") // Delete flight
            {
                Console.Write("Are you sure you want to delete this flight? [Y/N]: ");
                string? confirmation = Console.ReadLine()?.ToUpper();

                if (confirmation == "Y")
                {
                    flights.Remove(flightNumber);
                    flightSpecialRequests.Remove(flightNumber);
                    flightGateAssignments.Remove(flightNumber);
                    Console.WriteLine("Flight deleted successfully!");
                }
                else
                {
                    Console.WriteLine("Deletion cancelled.");
                    return;
                }
            }
            else
            {
                Console.WriteLine("Invalid choice.");
                return;
            }

            // Display updated flight details
            Console.WriteLine("\n=== Updated Flight Details ===");
            Console.WriteLine($"\nFlight Number: {selectedFlight.FlightNumber}");
            Console.WriteLine($"Airline Name: {selectedAirline.Name}");
            Console.WriteLine($"Origin: {selectedFlight.Origin}");
            Console.WriteLine($"Destination: {selectedFlight.Destination}");
            Console.WriteLine($"Expected Time: {selectedFlight.ExpectedTime:dd/MM/yyyy HH:mm}");
            Console.WriteLine($"Status: {selectedFlight.Status}");

            if (flightSpecialRequests.ContainsKey(selectedFlight.FlightNumber))
                Console.WriteLine($"Special Request Code: {flightSpecialRequests[selectedFlight.FlightNumber]}");

            if (flightGateAssignments.ContainsKey(selectedFlight.FlightNumber))
                Console.WriteLine($"Boarding Gate: {flightGateAssignments[selectedFlight.FlightNumber].GateName}");

            Console.WriteLine("-------------------");
        }


        // Process unassigned flights and attempt to assign a boarding gate.
        static void ProcessUnassignedFlights(Dictionary<string, Flight> flights, Dictionary<string, BoardingGate> gates)
        {
            Console.WriteLine("\n=== Processing Unassigned Flights ===\n");
            int processedFlights = 0;
            int successfulAssignments = 0;
            foreach (var flight in flights.Values)
            {
                if (!flightGateAssignments.ContainsKey(flight.FlightNumber))
                {
                    processedFlights++;
                    string specialRequest = flightSpecialRequests.ContainsKey(flight.FlightNumber)
                        ? flightSpecialRequests[flight.FlightNumber]
                        : "";
                    BoardingGate? assignedGate = null;
                    if (!string.IsNullOrEmpty(specialRequest))
                    {
                        assignedGate = FindMatchingGate(gates, specialRequest);
                    }
                    if (assignedGate == null)
                    {
                        assignedGate = FindAnyAvailableGate(gates);
                    }
                    if (assignedGate != null)
                    {
                        flightGateAssignments[flight.FlightNumber] = assignedGate;
                        assignedGate.Flight = flight;
                        successfulAssignments++;
                        DisplayFlightAssignment(flight, assignedGate);
                    }
                    else
                    {
                        Console.WriteLine($"No available gate for flight {flight.FlightNumber}");
                    }
                }
            }
            double percentage = processedFlights > 0 ? (successfulAssignments * 100.0 / processedFlights) : 0;
            Console.WriteLine($"\nProcessed {processedFlights} flights. Successful assignments: {successfulAssignments} ({percentage:F2}%).");
        }

        // Searches for a gate that supports the given special request code.
        static BoardingGate? FindMatchingGate(Dictionary<string, BoardingGate> gates, string specialRequestCode)
        {
            return gates.Values.FirstOrDefault(gate =>
                gate.Flight == null &&
                (
                    (specialRequestCode.Equals("CFFT", StringComparison.OrdinalIgnoreCase) && gate.SupportsCFFT) ||
                    (specialRequestCode.Equals("DDJB", StringComparison.OrdinalIgnoreCase) && gate.SupportsDDJB) ||
                    (specialRequestCode.Equals("LWTT", StringComparison.OrdinalIgnoreCase) && gate.SupportsLWTT)
                )
            );
        }

        // Returns any available gate (one with no flight assigned).
        static BoardingGate? FindAnyAvailableGate(Dictionary<string, BoardingGate> gates)
        {
            return gates.Values.FirstOrDefault(gate => gate.Flight == null);
        }

        // Display details about a flight assignment.
        static void DisplayFlightAssignment(Flight flight, BoardingGate gate)
        {
            string specialReq = flightSpecialRequests.ContainsKey(flight.FlightNumber)
                    ? flightSpecialRequests[flight.FlightNumber]
                    : "None";
            Console.WriteLine("\n--- Flight Assignment Details ---");
            Console.WriteLine($"Flight Number: {flight.FlightNumber}");
            Console.WriteLine($"Origin: {flight.Origin}");
            Console.WriteLine($"Destination: {flight.Destination}");
            Console.WriteLine($"Expected Time: {flight.ExpectedTime:dd/MM/yyyy HH:mm}");
            Console.WriteLine($"Special Request: {specialReq}");
            Console.WriteLine($"Assigned Gate: {gate.GateName}");
        }

        static void DisplayTotalFeesPerAirline(Dictionary<string, Airline> airlines, Dictionary<string, Flight> flights, Dictionary<string, BoardingGate> boardingGates, Dictionary<string, double> gateFees)
        {
            Terminal terminal = new Terminal("Terminal 5", airlines, flights, boardingGates, gateFees);
            terminal.PrintAirlineFees();
        }

        // BONUS FEATURE
        static void SearchFlightsInTimeWindow(Dictionary<string, Flight> flights)
        {
            Console.WriteLine("\n=== Flight Time Window Search ===\n");

            // Get time window from user
            Console.Write("Enter start time (HH:mm): ");
            if (!DateTime.TryParse(DateTime.Now.Date.ToString("d") + " " + Console.ReadLine(), out DateTime startTime))
            {
                Console.WriteLine("Invalid time format. Please use HH:mm");
                return;
            }

            Console.Write("Enter end time (HH:mm): ");
            if (!DateTime.TryParse(DateTime.Now.Date.ToString("d") + " " + Console.ReadLine(), out DateTime endTime))
            {
                Console.WriteLine("Invalid time format. Please use HH:mm");
                return;
            }

            // Find flights within the time window
            var flightsInWindow = flights.Values
                .Where(f => f.ExpectedTime.TimeOfDay >= startTime.TimeOfDay &&
                           f.ExpectedTime.TimeOfDay <= endTime.TimeOfDay)
                .OrderBy(f => f.ExpectedTime)
                .ToList();

            if (!flightsInWindow.Any())
            {
                Console.WriteLine($"\nNo flights found between {startTime:HH:mm} and {endTime:HH:mm}");
                return;
            }

            
            Console.WriteLine($"\nFound {flightsInWindow.Count} flights between {startTime:HH:mm} and {endTime:HH:mm}");

            Console.WriteLine("\nFlights in time window:");
            foreach (var flight in flightsInWindow)
            {
                Console.WriteLine($"Time: {flight.ExpectedTime:HH:mm} | Flight: {flight.FlightNumber} | " +
                                $"Origin: {flight.Origin} | Destination: {flight.Destination} | ");
            }
        }



        // BONUS FEATURE #2

        static void DetectScheduleConflicts(Dictionary<string, Flight> flights,
                                           Dictionary<string, BoardingGate> gates,
                                           Dictionary<string, BoardingGate> flightGateAssignments)
        {
            const int MIN_BUFFER_TIME = 30;
            Console.Clear();
            Console.WriteLine("=== Flight Schedule Conflict Detection System ===\n");

            // List to track conflicts
            var conflicts = new List<(Flight flight1, Flight flight2, BoardingGate gate, int overlapMinutes)>();
            int totalConflictsFound = 0;
            int conflictsResolved = 0;

            // Check each gate for potential conflicts by looking at consecutive flights
            foreach (var gate in gates.Values)
            {
                var gateFlights = flights.Values
                    .Where(f => flightGateAssignments.ContainsKey(f.FlightNumber) &&
                                flightGateAssignments[f.FlightNumber].GateName == gate.GateName)
                    .OrderBy(f => f.ExpectedTime)
                    .ToList();

                for (int i = 0; i < gateFlights.Count - 1; i++)
                {
                    var currentFlight = gateFlights[i];
                    var nextFlight = gateFlights[i + 1];

                    // Calculate the time difference between consecutive flights
                    var timeDiff = (nextFlight.ExpectedTime - currentFlight.ExpectedTime).TotalMinutes;
                    if (timeDiff < MIN_BUFFER_TIME)
                    {
                        conflicts.Add((currentFlight, nextFlight, gate, (int)(MIN_BUFFER_TIME - timeDiff)));
                        totalConflictsFound++;
                    }
                }
            }

            // Process found conflicts
            if (conflicts.Any())
            {
                Console.WriteLine($"Found {totalConflictsFound} scheduling conflict(s):\n");
                foreach (var conflict in conflicts)
                {
                    Console.WriteLine($"Conflict at Gate {conflict.gate.GateName}:");
                    Console.WriteLine($"  Flight {conflict.flight1.FlightNumber} at {conflict.flight1.ExpectedTime:HH:mm}");
                    Console.WriteLine($"  Flight {conflict.flight2.FlightNumber} at {conflict.flight2.ExpectedTime:HH:mm}");
                    Console.WriteLine($"  Buffer time needed: {conflict.overlapMinutes} minute(s)");
                    Console.WriteLine(new string('-', 40));

                    bool foundAlternative = false;
                    foreach (var potentialGate in gates.Values)
                    {
                        if (!HasConflictWithGate(conflict.flight2, potentialGate, flights, flightGateAssignments, MIN_BUFFER_TIME))
                        {
                            // Reassign the flight to the alternative gate
                            flightGateAssignments[conflict.flight2.FlightNumber] = potentialGate;
                            potentialGate.Flight = conflict.flight2;
                            Console.WriteLine($"Resolved: Flight {conflict.flight2.FlightNumber} reassigned to Gate {potentialGate.GateName}");
                            conflictsResolved++;
                            foundAlternative = true;
                            break;
                        }
                    }

                    if (!foundAlternative)
                    {
                        Console.WriteLine($"Unable to find an alternative gate for Flight {conflict.flight2.FlightNumber}");
                    }
                }

                DisplayConflictResolutionSummary(totalConflictsFound, conflictsResolved);
            }
            else
            {
                Console.WriteLine("No scheduling conflicts detected.");
            }
        }

        static bool HasConflictWithGate(Flight flight, BoardingGate gate, Dictionary<string, Flight> flights,
                                          Dictionary<string, BoardingGate> flightGateAssignments, int minBufferTime)
        {
            var gateFlights = flights.Values
                .Where(f => flightGateAssignments.ContainsKey(f.FlightNumber) &&
                            flightGateAssignments[f.FlightNumber].GateName == gate.GateName)
                .ToList();

            foreach (var existingFlight in gateFlights)
            {
                var timeDiff = Math.Abs((existingFlight.ExpectedTime - flight.ExpectedTime).TotalMinutes);
                if (timeDiff < minBufferTime)
                {
                    return true;
                }
            }
            return false;
        }

        static void DisplayConflictResolutionSummary(int totalConflicts, int resolved)
        {
            Console.WriteLine("\n=== Conflict Resolution Summary ===");
            Console.WriteLine($"Total conflicts detected: {totalConflicts}");
            Console.WriteLine($"Conflicts resolved: {resolved}");
            double resolutionRate = totalConflicts > 0 ? (resolved * 100.0 / totalConflicts) : 100;
            Console.WriteLine($"Resolution rate: {resolutionRate:F2}%");

            if (totalConflicts > resolved)
            {
                Console.WriteLine("\nManual intervention required for unresolved conflicts.");
                Console.WriteLine("Suggestions:");
                Console.WriteLine("1. Adjust flight times");
                Console.WriteLine("2. Consider temporary gates");
                Console.WriteLine("3. Review special requirements");
            }
        }

    }
        }