namespace ElevatorAPI.Models
{
    public enum ElevatorActionStage
    {
        Begin,
        EnsuringDoorClosed,
        ReachingOriginFloor,
        OpeningBoardingDoor,
        WaitingToBoard,
        ClosingBoardingDoor,
        ReachingDestinationFloor,
        OpeningAlightingDoor,
        WaitingToAlight,
        ClosingAlightingDoor,
        End
    }
}
