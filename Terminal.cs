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

    public void PrintAirlineFees() { }

    public override string ToString()
    {
        return "Terminal: " + TerminalName;
    }
}