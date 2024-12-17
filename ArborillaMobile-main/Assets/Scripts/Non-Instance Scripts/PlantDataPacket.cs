using Newtonsoft.Json;
using System;

/// <summary>
/// Dati identificativi della pianta
/// </summary>
[Serializable]
public class PlantDataPacket
{
    public readonly string spotName;
    public readonly PlantGenetics.AllelesCouple chromosomes;
    public PlantGenetics.AllelesCouple sonChromosomes;
    public PlantScript.LifeStage lifeStage;
    public short stageTime;
    public string time;

    [JsonConstructor]
    public PlantDataPacket(     //usato per la deserializzazione dei dati
        string spotName, 
        PlantGenetics.AllelesCouple chromosomes,
        PlantGenetics.AllelesCouple sonChromosomes, 
        PlantScript.LifeStage lifeStage, 
        short stageTime, string time
        )
    {
        this.spotName = spotName;
        this.chromosomes = chromosomes;
        this.sonChromosomes = sonChromosomes;
        this.lifeStage = lifeStage;
        this.stageTime = stageTime;
        this.time = time;
    }

    public PlantDataPacket(string spotName, PlantGenetics genetics)
    {
        this.spotName = spotName;
        chromosomes = genetics.chromosomes;
        sonChromosomes = PlantGenetics.AllelesCouple.none;
        lifeStage = PlantScript.LifeStage.Growing;
        stageTime = genetics.defaultTimeToGrow;
        time = DateTime.Now.ToString();
    }

    public override string ToString()
    {
        return $"Nome spot: {spotName}\n" +
            $"cromosomi: {chromosomes}\n" +
            $"cromosomi frutto: {sonChromosomes}\n" +
            $"life stage: {lifeStage}\n" +
            $"stage time:{stageTime}\n" +
            $"last save time: {time}\n";
    }
}
