//==========================================================
// Student Number : S10266942
// Student Name :  Pugazhenthi Dharundev
// Partner Name : T Venkatesh
//==========================================================


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace s10266695_s10266942_prg2_assignment;

internal class Terminal
{
    private string terminalName;
    public string TerminalName
    {
        get { return terminalName; }
        set { terminalName = value; }
    }

    private Dictionary<String, Airline> airlines;
    public Dictionary<String, Airline> Airlines
    {
        get { return airlines; }
        set { airlines = value; }
    }

    private Dictionary<String, Flight> flights;
    public Dictionary<String, Flight> Flights
    {
        get { return flights; }
        set { flights = value; }
    }

    private Dictionary<String, BoardingGate> boardingGates;
    public Dictionary<String, BoardingGate> BoardingGates
    {
        get { return boardingGates; }
        set { boardingGates = value; }
    }

    private Dictionary<String, double> gateFees;
    public Dictionary<String, double> GateFees
    {
        get { return gateFees; }
        set { gateFees = value; }
    }

    public Terminal(string terminalName, Dictionary<String, Airline> airlines, Dictionary<String, Flight> flights, Dictionary<String, BoardingGate> boardingGates, Dictionary<String, double> gateFees)
    {
        TerminalName = terminalName;
        Airlines = airlines;
        Flights = flights;
        BoardingGates = boardingGates;
        GateFees = gateFees;
    }

    public bool AddAirline(Airline airline)
    {
        if (Airlines.ContainsKey(airline.Code))
        {
            return false;
        }
        else
        {
            Airlines.Add(airline.Code, airline);
            return true;
        }
    }


    public bool AddBoardingGate(BoardingGate boardingGate)
    {
        if (BoardingGates.ContainsKey(boardingGate.GateName))
        {
            return false;
        }
        else
        {
            BoardingGates.Add(boardingGate.GateName, boardingGate);
            return true;
        }
    }

    public Airline GetAirlineFromFlight(Flight flight)
    {
        foreach (Airline airline in Airlines.Values)
        {
            if (airline.Flights.ContainsValue(flight))
            {
                return airline;
            }
        }
        return null;
    }


    public double PrintAirlineFees()
    {
        // Track totals across all airlines
        double totalTerminalFees = 0;

        // Process each airline separately
        foreach (var airline in Airlines.Values)
        {
            double totalFees = 0;
            double totalDiscounts = 0;
            int arrivingFlights = 0;
            int departingFlights = 0;
            int flightsWithoutSpecialRequest = 0;

            // Get flights for this airline
            var airlineFlights = flights.Values.Where(f => f.FlightNumber.StartsWith(airline.Code));

            foreach (var flight in airlineFlights)
            {
                double flightFee = 0;

                // Apply Boarding Gate Base Fee
                flightFee += 300;

                // Check if Origin or Destination is Singapore (SIN)
                if (flight.Origin == "SIN")
                {
                    flightFee += 800; // Departing flight fee
                    departingFlights++;
                }
                if (flight.Destination == "SIN")
                {
                    flightFee += 500; // Arriving flight fee
                    arrivingFlights++;
                }

                // Check for Special Request Code and apply respective fee
                if (flight is CFFTFlight)
                {
                    flightFee += 150;
                }
                else if (flight is DDJBFlight)
                {
                    flightFee += 300;
                }
                else if (flight is LWTTFlight)
                {
                    flightFee += 500;
                }
                else
                {
                    flightsWithoutSpecialRequest++;
                }

                // Add this flight's fee to total
                totalFees += flightFee;
            }

            // Apply Promotional Discounts
            if (arrivingFlights >= 3)
            {
                totalDiscounts += 350;
            }
            if (departingFlights >= 3)
            {
                totalDiscounts += 350;
            }
            if (flightsWithoutSpecialRequest > 0)
            {
                totalDiscounts += flightsWithoutSpecialRequest * 50;
            }

            var originsToCheck = new[] { "DXB", "BKK", "NRT" };
            if (airlineFlights.Any(f => originsToCheck.Contains(f.Origin)))
            {
                totalDiscounts += 25;
            }

            if (airlineFlights.Count() >= 5)
            {
                totalDiscounts += totalFees * 0.03; // 3% discount on total fees
            }

            // Final amount after discount for this airline
            double finalTotal = totalFees - totalDiscounts;

            // Display breakdown for this airline
            Console.WriteLine($"Airline: {airline.Name} ({airline.Code})");
            Console.WriteLine($"Subtotal Fees: ${totalFees:F2}");
            Console.WriteLine($"Total Discounts: ${totalDiscounts:F2}");
            Console.WriteLine($"Final Total Fees: ${finalTotal:F2}");
            Console.WriteLine("-------------------------------------------------");

            totalTerminalFees += finalTotal;
        }

        Console.WriteLine($"\nTotal Terminal Fees: ${totalTerminalFees:F2}");
        return totalTerminalFees;
    }
}