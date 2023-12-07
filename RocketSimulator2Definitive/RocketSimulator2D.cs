using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using UnityEngine;
using System;
using System.Diagnostics;
using System.ComponentModel;
using Unity.Collections;
using System.Net.Sockets;

class UIJob : IJob
{
    public void Execute()
    {
        ExecutarMusica();
        TimeCount();
    }

    private static  void ExecutarMusica()
    {
        UnityEngine.Debug.Log("Essa fun��o executa a m�sica do jogo de fundo");
    }

    private static void TimeCount()
    {
        UnityEngine.Debug.Log("Essa Fun��o mostra a contagem do tempo na tela");
    }
}

public struct massRocketJob : IJobParallelFor
{
    public NativeArray<Rocket> rockets;
    public void Execute(int index)
    {
        definirRazaoMassa(rockets[index]);
    }
    private static void definirRazaoMassa(Rocket rocket)
    {
        rocket.massFinalRocket = rocket.massRocket + rocket.massFuel;//Massa total = massa foguete + massa combust�vel

    }
}

public struct empuxoRocketJob : IJobParallelFor
{
    public NativeArray<Rocket> rockets;
    public void Execute(int index)
    {
        definirEmpuxoAtual(rockets[index]);
    }

    static void definirEmpuxoAtual(Rocket rocket)
    {
        rocket.empuxoAtual = Math.Min(rocket.empuxoM�ximo, rocket.massFuel / rocket.taxFuel); //Retorna o menor valor entre o empuxo m�ximo ou a raz�o entre a massa do combustivel pela taxa de fluxo de massa

    }
}

public class RocketSimulator2D : MonoBehaviour
{
    void Start()
    {
        int time = 0;
        var perfomanceMeasure = new Stopwatch(); //inst�ncia do cron�metro
        List<Rocket> rockets = new List<Rocket>()
        {
            new Rocket("Saturn V", 1710000, 2829000, 0.1 * Math.Pow(10,5),33.375 *Math.Pow(10,3)),
            new Rocket("Space Shuttle",78000, 2040000, 5775, 5777 * Math.Pow(10, 3))  //declara nome, peso sem combust�vel, peso do combust�vel, taxa de fluxo de massa, empuxo m�ximo
        };
        List<Planeta> planetas = new List<Planeta>()
        {
            new Planeta("Terra", 9.8, 6370 * Math.Pow(10, 3)),//declara nome, gravidade, raio
            new Planeta("Marte", 3.72, 3389 * Math.Pow(10,3))
        };

        NativeArray<Rocket> rocketArray = new(rockets.ToArray(), Allocator.TempJob);

        massRocketJob massJob = new massRocketJob
        {
            rockets = rocketArray
        };
        JobHandle massJobHandle = massJob.Schedule(rockets.Count, 64);

        massJobHandle.Complete();

        empuxoRocketJob empuxoJob = new empuxoRocketJob
        {
            rockets = rocketArray
        };
        JobHandle empuxoJobHandle = empuxoJob.Schedule(rockets.Count, 64);
        empuxoJobHandle.Complete();

        rocketArray.Dispose();

        UnityEngine.Debug.Log("SIMULADOR DE FOGUETE 2D");
        UnityEngine.Debug.Log("-----------------------");
        UnityEngine.Debug.Log("Selecione seu Foguete:");

        for (int i = 0; i < rockets.Count; i++)
        {
            UnityEngine.Debug.Log($"{i + 1} - {rockets[i].nome}");
        }

        int currentRocket = 0; // int.Parse(Console.ReadLine()) - 1;
        UnityEngine.Debug.Log($"Foguete Selecionado: {rockets[currentRocket].nome}");

        UnityEngine.Debug.Log("Selecione o planeta de simula��o:");
        for (int j = 0; j < planetas.Count; j++)
        {
            UnityEngine.Debug.Log($"{j + 1} - {planetas[j].nome}");
        }

        int currentPlanet = 0; // int.Parse(Console.ReadLine()) - 1;
        UnityEngine.Debug.Log($"Planeta Selecionado: {planetas[currentPlanet].nome}");

        perfomanceMeasure.Start();

        while (time < 10 && rockets[currentRocket].posY < planetas[currentPlanet].raio)
        {
            UnityEngine.Debug.Log($"Tempo: {time} segundos");
            UnityEngine.Debug.Log($"Posi��o do Foguete: {rockets[currentRocket].posY}");
            UnityEngine.Debug.Log($"Combust�vel restante: {rockets[currentRocket].massFuel}");
            AtualizarStatus(rockets[currentRocket], planetas[currentPlanet]);
            UIJob uiJ = new UIJob();
            uiJ.Execute();
            if (rockets[currentRocket].posY >= planetas[currentPlanet].raio)
            {
                UnityEngine.Debug.Log("O Foguete saiu do Planeta com sucesso!");
            }

            definirRazaoMassa(rockets[currentRocket]);
            definirEmpuxoAtual(rockets[currentRocket]);

            UnityEngine.Debug.Log("");

            time++;
        }

        perfomanceMeasure.Stop();
        UnityEngine.Debug.Log($"Tempo decorrido: {perfomanceMeasure.Elapsed}");
    }
    static void definirRazaoMassa(Rocket rocket)
    {
        rocket.massFinalRocket = rocket.massRocket + rocket.massFuel;//Massa total = massa foguete + massa combust�vel
       
    }

    static void definirEmpuxoAtual(Rocket rocket)
    {
        rocket.empuxoAtual = Math.Min(rocket.empuxoM�ximo, rocket.massFuel / rocket.taxFuel); //Retorna o menor valor entre o empuxo m�ximo ou a raz�o entre a massa do combustivel pela taxa de fluxo de massa
       
    }

    static double aceleracao(Rocket rocket, Planeta planeta)
    {
        return (rocket.empuxoAtual - rocket.massFinalRocket * planeta.gravidade) / rocket.massFinalRocket; //retorna a acelera��o
        //empuxo - peso = empuxo - massa * gravidade do planeta
    }
    static void AtualizarStatus(Rocket rocket, Planeta planeta)
    {
        rocket.posY += Math.Abs(rocket.velocidade); //A posi��o do foguete atualiza com a velocidade atual
        rocket.velocidade += aceleracao(rocket, planeta);//A velocidade � aumentada pelo aumento de aceleracao
        rocket.massFuel -= rocket.taxFuel;//A cada segundo o combustivel vai diminuindo, subtraindo a massa pela taxa de fluxo de massa
    }

}


public struct Rocket
{
    public string nome { get; set; } //nome do foguete
    public double massRocket { get; set; } //massa do foguete sem combust�vel
    public double massFuel { get; set; }//massa do combust�vel
    public double posY { get; set; }//posi��o do foguete no eixo y
    public double taxFuel { get; set; }
    public double massFinalRocket { get; set; }
    public double empuxoM�ximo { get; set; }
    public double empuxoAtual { get; set; }
    public double velocidade { get; set; }

    public Rocket(string Nome, double MR, double MF, double Fuel, double MaxE)
    {
        nome = Nome;
        massRocket = MR;
        massFuel = MF;
        posY = 0;
        taxFuel = Fuel;
        empuxoM�ximo = MaxE;
        velocidade = 0;
        massFinalRocket = 0;
        empuxoAtual = 0;
    }
}

class Planeta
{
    public string nome { get; set; }
    public double gravidade { get; set; }
    public double raio { get; set; }


    public Planeta(string Nome, double G, double R)
    {
        nome = Nome;
        gravidade = G;
        raio = R;
    }
}
