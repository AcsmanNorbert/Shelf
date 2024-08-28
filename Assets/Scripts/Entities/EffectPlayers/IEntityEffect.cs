public enum EntityEvent { Attack, Alert, Dead }

public interface IEntityEffect
{
    public void DoEffect(EntityEvent entityEvent);
}
