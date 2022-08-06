public interface IStationStateSwitcher
{
    public void SwitchState<T>() where T : BaseSheepState;
}