using System;
using System.Collections.Generic;
using System.Linq;

// 主题接口
public interface ISubject
{
    void RegisterObserver(IObserver observer);
    void RemoveObserver(IObserver observer);
    void NotifyObservers(string news);
}

// 观察者接口
public interface IObserver
{
    void Update(string news);
}

// 具体主题
public class NewsSubject : ISubject
{
    private List<IObserver> observers = new List<IObserver>();
    private string latestNews;

    public void RegisterObserver(IObserver observer)
    {
        observers.Add(observer);
    }

    public void RemoveObserver(IObserver observer)
    {
        observers.Remove(observer);
    }

    public void NotifyObservers(string news)
    {
        latestNews = news;
        foreach (var observer in observers)
        {
            observer.Update(latestNews);
        }
    }

    public void PublishNews(string news)
    {
        Console.WriteLine($"Breaking News: {news}");
        NotifyObservers(news);
    }
}

// 具体观察者
public class NewsSubscriber : IObserver
{
    private string subscriberName;

    public NewsSubscriber(string name)
    {
        subscriberName = name;
    }

    public void Update(string news)
    {
        Console.WriteLine($"{subscriberName} received news: {news}");
    }
}

class Program
{
    static void Main()
    {
        //NewsSubject newsSubject = new NewsSubject();

        //NewsSubscriber subscriber1 = new NewsSubscriber("Subscriber 1");
        //NewsSubscriber subscriber2 = new NewsSubscriber("Subscriber 2");

        //newsSubject.RegisterObserver(subscriber1);
        //newsSubject.RegisterObserver(subscriber2);
        //newsSubject.RemoveObserver(subscriber2);

        //newsSubject.PublishNews("Important Event!"); // This will notify both subscribers

        var ok = new List<string>();
        ok.Add("3444");
        var ment = "1234";
        var CourseIgnores = ment.Contains(",") ? ment.Split(',').ToArray()
                            :new string[] { ment };

        var IgnoresList = CourseIgnores.ToList();

        // 增加免考课程
        IgnoresList.AddRange(ok.Select(d => d));
    }
}
