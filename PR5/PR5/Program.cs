var builder = WebApplication.CreateBuilder();
var app = builder.Build();

List<Person> people = new List <Person>(100);
int yourid = 0;
int id = 0;
var now = DateTime.Now;
int count = 0;
int exit = 0;
int[] delete = new int[100];
app.Run(async (context) =>
{
    context.Response.ContentType = "text/html; charset=utf-8";
    if (context.Request.Path == "/signup")
    {
        id++;
        var form = context.Request.Form;
        people.Add(new Person() { Name = form["name"], Password = form["Password"], Email = form["email"], Id = id, Date=now.ToShortDateString(), Time="-", Status="Offline"});
        await context.Response.SendFileAsync("html/signin.html");
    }
    else if(context.Request.Path == "/delete")
    {
        exit = 0;
        var del = context.Request.Form;
        for (int j=0; j<100; j++)
        { 
            if (del[$"id{j}"] == "on")
            {
                var itemToRemove = people.Single(r => r.Id == j);
                people.Remove(itemToRemove);
                if (j == yourid)
                {
                    exit = 1;
                }
            }
        }
        if(exit == 1)
        {
            context.Response.Redirect("/signin");
        }
        else
        {
            context.Response.Redirect("/main");
        }
                      
    }   
    else if (context.Request.Path == "/main")
    {
        await context.Response.SendFileAsync("html/users.html");
        var stringBuilder = new System.Text.StringBuilder("<tbody>");
        foreach (var p in people)
        {
            stringBuilder.Append($"<tr><th scope='row'><input type='checkbox' class='thing' name='id{p.Id}'/></th><td>{p.Id}</td><td>{p.Name}</td><td>{p.Email}</td><td>{p.Date}</td><td>{p.Time}</td><td>{p.Status}</td></tr>");
        }
        stringBuilder.Append("</tbody></table></form>");
        stringBuilder.Append("<script>var checkboxes = document.querySelectorAll('input.thing'),checkall = document.getElementById('checkall');for(var i=0; i<checkboxes.length; i++){checkboxes[i].onclick = function(){var checkedCount = document.querySelectorAll('input.thing:checked').length;checkall.checked = checkedCount > 0;checkall.indeterminate = checkedCount > 0 && checkedCount < checkboxes.length;}}checkall.onclick = function(){for(var i=0; i<checkboxes.length; i++){checkboxes[i].checked = this.checked;}}</script>");
        await context.Response.WriteAsync(stringBuilder.ToString());
        count = id;
    }
    else if (context.Request.Path == "/signin")
    {
        await context.Response.SendFileAsync("html/signin.html");
    }

    else if (context.Request.Path == "/sign")
    {
        var form = context.Request.Form;
        if (people.Exists(x => x.Email == form["email"]) && people.Exists(x => x.Password == form["password"]))
        {
            var nd = DateTime.Now;
            var user = people.Single(a => a.Email == form["email"]);
            yourid = user.Id;
            people.FindAll(s => s.Id == (yourid)).ForEach(x => { x.Status = "Online"; x.Time = nd.ToShortTimeString(); });
            context.Response.Redirect("/main");
        }
        else
        {
            await context.Response.SendFileAsync("html/signup.html");
        }
    }
    else
    {
        await context.Response.SendFileAsync("html/signup.html");
    }
});
app.Run();
public class Person
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Password { get; set; }
    public string? Email { get; set; }
    public object? Date { get; set; }
    public object? Time { get; set; }
    public string? Status { get; set; }
    public bool? Block { get; set; }
}