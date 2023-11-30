using Unity.Collections;
using System;
using Unity.Engine;
using Unity.Jobs;


public struct RocketJob : IJob
{
    public Rocket rocket;
    public Planeta planeta;
    public float deltaTime;

    public void Execute()
    {
        definirRazaoMassa(ref rocket); //Função de Atualizar a Massa Final do Foguete e a Massa do Combustível;
        definirEmpuxoAtual(ref rocket, ref planeta); //Atualizar Impulso atual 
        AtualizarStatus(ref rocket, planeta, deltaTime); //Atualização de Velocidade, Aceleração, Massa do Combustível
    }
    
}

public class UnityJobsRocketSimulator : MonoBehaviour
{
    private void Start():
    {
        int time = 0;
        var perfomanceMeasure = new Stopwatch(); //instância do cronômetro
        perfomanceMeasure.Start();//inicio do cronômetro
        List<Rocket> rockets = new List<Rocket>()
            {
                new Rocket("Saturn V", 1710000, 2829000, 0.1 * Math.Pow(10,5),33.375 *Math.Pow(10,3)) //declara nome, peso sem combustível, peso do combustível, taxa de fluxo de massa, empuxo máximo
            };
        List<Planeta> planetas = new List<Planeta>()
            {
                new Planeta("Terra", 9.8, 6370 * Math.Pow(10, 3))//declara nome, gravidade, raio
            };
        NativeArray<Rocket> rocketsArray = new NativeArray<Rocket>(rockets.ToArray(), Allocator.TempJob); //Instancia da Array de todos os rockets, e chama um alocador temporário para memória da array
        Planeta currentPlaneta = planetas[0]; //define o planeta atual como sendo o primeiro da lista(terra)
        RocketJob rocketjob = new RocketJob();//Instância do Job das funções utilizadas
        while(time < 10 && rocketsArray[0].posY  < currentPlaneta.raio)
        {
            Debug.Log($"Tempo: {time} segundos");
            Debug.Log($"Posição do Foguete: {rocketsArray[0].posY} metros");
            Debug.Log($"Combustível Restante: {rocketsArray[0].massFuel} kg");
            AtualizarStatus(ref rocketsArray[0], currentPlaneta, deltaTime);
            rocketJob.rocket = rocketsArray[0]; //Atribui o rocket do rocketJob ao foguete da lista;
            rocketJob.planeta = currentPlaneta;
            rocketJob.deltaTime = Time.deltaTime;

            rocketJob.Schedule().complete();

            if(rocketsArray.[0].posY >= currentPlaneta.raio)
            {
                Debug.Log("O Foguete saiu do Planeta com sucesso!");
            }

            definirRazaoMassa(ref rocketsArray[0]);
            definirEmpuxoAtual(ref rocketsArray[0], currentPlaneta);
            time++
        }

        perfomanceMeasure.Stop();
        Debug.Log($"Tempo decorrido: {perfomanceMeasure.Elapsed}");

        rocketsArray.Dispose();
    }
    private static void definirRazaoMassa(ref Rocket rocket)
    {
        rocket.massFinalRocket = rocket.massRocket + rocket.massFuel * deltaTime;//Massa total = massa foguete + massa combustível
   
    }

    private static void definirEmpuxoAtual(ref Rocket rocket, Planeta planeta)
    {
        rocket.empuxoAtual = Math.Min(rocket.empuxoMáximo, rocket.massFuel / rocket.taxFuel) * deltaTime;//Retorna o menor valor entre o empuxo máximo ou a razão entre a massa do combustivel pela taxa de fluxo de massa
       
    }

    private static double aceleracao(Rocket rocket, Planeta planeta)
    {
        return (rocket.empuxoAtual - rocket.massFinalRocket * planeta.gravidade) / rocket.massFinalRocket * deltaTime;//retorna a aceleração
        //empuxo - peso = empuxo - massa * gravidade do planeta
    }
    
    private static void AtualizarStatus(ref Rocket rocket, Planeta planeta, float deltaTime)
    {
        rocket.posY += Math.Abs(rocket.velocidade) * deltaTime; //A posição do foguete atualiza com a velocidade atual
        rocket.velocidade += aceleracao(rocket, planeta) * deltaTime;//A velocidade é aumentada pelo aumento de aceleracao
        rocket.massFuel -= rocket.taxFuel * deltaTime;//A cada segundo o combustivel vai diminuindo, subtraindo a massa pela taxa de fluxo de massa
        
    }
}