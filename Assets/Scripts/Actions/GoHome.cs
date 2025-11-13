public class GoHome : GoapAction
{
    public override bool PrePerform()
    {
        return true;
    }
    public override bool PostPerform()
    {
        Destroy(gameObject, 3f);
        return true;
    }
}
