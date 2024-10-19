using static Boot.Bootstrap;

namespace Boot
{
    public interface IBoot
    {
        void InitAwake();

        void InitStart();

        (TypeLoadObject typeLoad, TypeSingleOrLotsOf singleOrLotsOf) GetTypeLoad();
    }
}
