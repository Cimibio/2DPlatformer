using System;

public interface IPickupable
{
    public event Action<IPickupable> PickedUp;

    public void PickUp();
}