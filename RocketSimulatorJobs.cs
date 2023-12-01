using Unity.Collections;
using System;
using UnityEngine;
using Unity.Jobs;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
using System.Numerics;
using System.Threading;

public struct RocketStatusAtualizationJob : IJob
{
    public float deltaTime;
    public Rocket rocket;
    public Planeta planeta;
    public void Execute()
    {
        AtualizarStatus(ref rocket, planeta, deltaTime);
    }
    private void AtualizarStatus(ref Rocket rocket, Planeta planeta, float deltaTime)
    {
        rocket.posY += Math.Abs(rocket.velocidade) * deltaTime;
        rocket.velocidade += Aceleracao(rocket, planeta) * deltaTime;
        rocket.massFuel -= rocket.taxFuel * deltaTime;
    }
    private double Aceleracao(Rocket rocket, Planeta planeta)
    {
        return (rocket.empuxoAtual - rocket.massFinalRocket * planeta.gravidade) / rocket.massFinalRocket;
    }
}

public struct RocketMassDefinitionJob : IJob
{
    public Rocket rocket;
    public void Execute()
    {
        DefinirRazaoMassa(ref rocket);
    }
    private void DefinirRazaoMassa(ref Rocket rocket)
    {
        rocket.massFinalRocket = rocket.massRocket + rocket.massFuel;
    }

}

public struct RocketEmpuxoDefinitionJob : IJob
{
    public Rocket rocket;
    public void Execute()
    {
        DefinirEmpuxoAtual(ref rocket);
    }

    private void DefinirEmpuxoAtual(ref Rocket rocket)
    {
        rocket.empuxoAtual = Math.Min(rocket.empuxoMáximo, rocket.massFuel / rocket.taxFuel);
    }
}

public struct UI_Job : IJob
{
    public float deltaTime;
    public void Execute()
    {
        ExecutarMúsica();
        TimeCount();
    }

    private void ExecutarMúsica()
    {
        UnityEngine.Debug.Log("Essa função executa a música do jogo de fundo");
    }

    private void TimeCount()
    {
       UnityEngine.Debug.Log("Essa Função mostra a contagem do tempo na tela");
    }


}
public class UnityJobsRocketSimulator : MonoBehaviour
{
    public void Start()
    {
        int time = 0;
        float deltaTime = Time.deltaTime;
        Stopwatch perfomanceMeasure = new();
        perfomanceMeasure.Start();

        List<Rocket> rockets = new()
        {
            new Rocket("Saturn V", 1710000, 2829000, 0.1 * Math.Pow(10,5),33.375 *Math.Pow(10,3))
        };

        List<Planeta> planetas = new()
        {
            new Planeta("Terra", 9.8, 6370 * Math.Pow(10, 3))
        };

        NativeArray<Rocket> rocketsArray = new(rockets.ToArray(), Allocator.TempJob);
        Planeta currentPlaneta = planetas[0];
        Rocket currentRocket = rockets[0];

        RocketStatusAtualizationJob rocketStatusJ = new();
        RocketMassDefinitionJob rocketMassJ = new();
        RocketEmpuxoDefinitionJob rocketEmpJ = new();
        UI_Job uiJ = new();

        while (time < 10 && rocketsArray[0].posY < currentPlaneta.raio)
        {
            UnityEngine.Debug.Log($"Tempo: {time} segundos");
            UnityEngine.Debug.Log($"Posição do Foguete: {currentRocket.posY} metros");
            UnityEngine.Debug.Log($"Combustível Restante: {currentRocket.massFuel} kg");
            uiJ.Schedule().Complete();

            rocketEmpJ.rocket = rocketsArray[0];
            rocketMassJ.rocket = rocketsArray[0];
            rocketStatusJ.rocket = rocketsArray[0];
            rocketStatusJ.planeta = currentPlaneta;
            rocketStatusJ.deltaTime = deltaTime;


            rocketStatusJ.Schedule().Complete();
            rocketMassJ.Schedule().Complete();
            rocketEmpJ.Schedule().Complete();

            if (rocketsArray[0].posY >= currentPlaneta.raio)
            {
                UnityEngine.Debug.Log("O Foguete saiu do Planeta com sucesso!");
            }

            time++;
        }

        perfomanceMeasure.Stop();
        UnityEngine.Debug.Log($"Tempo decorrido: {perfomanceMeasure.Elapsed}");

        rocketsArray.Dispose();
    }

}


public struct Rocket
{
    public string name;
    public double massRocket;
    public double massFuel;
    public double taxFuel;
    public double empuxoMáximo;
    public double posY;
    public double velocidade;
    public double massFinalRocket;
    public double empuxoAtual;
    public Rocket(string name, double massRocket, double massFuel, double taxFuel, double empuxoMáximo)
    {
          this.name = name;
          this.massRocket = massRocket;
          this.massFuel = massFuel;
          this.taxFuel = taxFuel;
          this.empuxoMáximo = empuxoMáximo;
          this.posY = 0.0;
          this.velocidade = 0.0;
          this.massFinalRocket = 0.0;
          this.empuxoAtual = 0.0;
    }

}

public struct Planeta
{
    public string name;
    public double gravidade;
    public double raio;

   public Planeta(string name, double gravidade, double raio)
   {
        this.name = name;
        this.gravidade = gravidade;
        this.raio = raio;
   }
}


