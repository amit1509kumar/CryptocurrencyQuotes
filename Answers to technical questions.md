
# Answers to Technical Questions

## 1. How long did you spend on the coding assignment? What would you add to your solution if you had more time?

I spent approximately 8 hours on the coding assignment. If I had more time, I would focus on the following improvements:
- **Enhanced Security:** Force clients to include JWT in the Authorization header for all API calls.
- **Integration Tests:** Write integration and load tests to ensure the solution works as expected under different scenarios.
- **Documentation:** Include detailed documentation to explain the code structure, API endpoints, and usage instructions.
- **Caching:** Implement caching to reduce the frequency of external API calls and provide a failover mechanism.
- **Monitoring:** Set up monitoring to collect usage data and track API performance.

---

## 2. What was the most useful feature that was added to the latest version of your language of choice?

One of the most useful features in the latest version of C# is the **`required` modifier** for properties, introduced in C# 11. It ensures that required properties must be initialized during object instantiation.

### Code Snippet:
```csharp
public class CurrencyDto
{
    public required string Currency { get; set; }
    public required decimal Price { get; set; }
}
```

---

## 3. How would you track down a performance issue in production? Have you ever had to do this?

Yes, I have used various tools in the past to track performance issues.

- **ElasticSearch and Kibana:** For application logs and setting up alerts.
- **Grafana:** To display performance metrics on a dashboard.
- **Load Testing (K6):** I have used K6 for load testing to simulate traffic and identify bottlenecks in production.

---

## 4. What was the latest technical book you have read or tech conference you have been to? What did you learn?

The latest technical book I read was *"Designing Data-Intensive Applications"* by Martin Kleppmann. I learned:
- Key concepts in distributed systems and their role in scaling applications.
- Techniques for achieving fault tolerance and ensuring data consistency across distributed systems.
- Different types of storage systems and their trade-offs in real-world applications.

---

## 5. What do you think about this technical assessment?

The assessment was good to evaluate API development skills and third-party API integration knowledge.

---

## 6. Please, describe yourself using JSON.

```json
{
  "name": "Amit Kumar",
  "role": "Senior Software Engineer",
  "skills": [
    "C#",
    "ASP.NET Core",
    "SQL",
    "Microservices",
    "Distributed Systems"
  ],
  "interests": [
    "APIs",
    "AI",
    "Distributed Systems",
    "Technical Mentorship"
  ],
  "latest_read": "Designing Data-Intensive Applications",
  "location": "Netherlands"
}
```
