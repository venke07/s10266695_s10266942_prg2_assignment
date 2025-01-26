using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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



    public Terminal() { }

    public Terminal(string terminalName, Dictionary<String, Airline> airlines, Dictionary<String, Flight> flights, Dictionary<String, BoardingGate> boardingGates, Dictionary<String, double> gateFees)
    {
        TerminalName = terminalName;
        Airlines = airlines;
        Flights = flights;
        BoardingGates = boardingGates;
        GateFees = gateFees;
    }

    public void AddAirline(Airline airline)
    {
        Airlines.Add(airline.Code, airline);
    }

    public void AddBoardingGate(BoardingGate boardingGate)
    {
        BoardingGates.Add(boardingGate.GateName, boardingGate);
    }

    public void GetAirlineFromFLight(Flight flight)
    {
        foreach (Airline airline in Airlines.Values)
        {
            if (airline.Flights.ContainsValue(flight))
            {
                Console.WriteLine(airline);
            }
        }
    }

    public void PrintAirlineFees()
    {

    }

    public override string ToString()
    {
        return "Terminal: " + TerminalName;
    }

}
