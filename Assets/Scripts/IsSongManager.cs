

public interface IsSongManager
{
    bool startStatus();
    float getBeatsPosition();
    float bpm { get; }
    float noteTravelBeats { get; }
    HoldNoteController activeHoldNote { get;set;}
}
