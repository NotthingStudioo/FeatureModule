namespace Game.Scripts.Shop.Condition
{
    public interface IShopCondition
    {
        string Id { get; }
        bool IsMet(string param);
    }
}