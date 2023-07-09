namespace GalE;

public class CheckInit
{
    private bool Inited = false;
    public void Init() => Inited = true;

    public void Check() => Check(false);

    public void Check(bool checkEd)
    {
        if(checkEd)
        {
            if(Inited)
            {
                throw new Exception("Already Inited");
            }
        }
        else if(Inited)
        {
            return;
        }
        else
        {
            throw new Exception("You need to call Init() first.");
        }
    }
}