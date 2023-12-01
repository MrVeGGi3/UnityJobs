//Simulador de Foguete em duas dimensões, promovendo o lançamento vetorial em Y para cima.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Models;
using Simulators;

class Program
{
    static void Main()
    {
        var perfomanceMeasure = new Stopwatch(); //instância do cronômetro
        perfomanceMeasure.Start();//inicio do cronômetro

        var simulator = new RocketSimulatorWithoutParallelism(new List<Rocket>()
        {
            new Rocket("Saturn V", 1710000, 2829000, 0.1 * Math.Pow(10,5),33.375 *Math.Pow(10,3)) //declara nome, peso sem combustível, peso do combustível, taxa de fluxo de massa, empuxo máximo
        }, new List<Planeta>()
        {
            new Planeta("Terra", 9.8, 6370 * Math.Pow(10, 3))//declara nome, gravidade, raio
        });

        simulator.run();

        perfomanceMeasure.Stop();
        Console.WriteLine($"Tempo decorrido: {perfomanceMeasure.Elapsed}");
    }
}
