using System;

/// <summary>
/// Dati identificativi della pianta
/// </summary>
[Serializable]
public class PlantDataPacket
{
    public readonly string spotName, time;
    public readonly PlantGenetics.AllelesCouple chromosomes, sonChromosomes;
    public readonly PlantScript.LifeStage lifeStage;
    public readonly short stageTime;

    public PlantDataPacket(
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

    public override string ToString()
    {
        return $"Nome spot: {spotName}\n" +
            $"cromosomi: {chromosomes}\n" +
            $"cromosomi frutto: {sonChromosomes}\n" +
            $"life stage: {lifeStage}\n" +
            $"stage time:{stageTime}\n" +
            $"last save time: {time}";
    }
}
