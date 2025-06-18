<!DOCTYPE html>
<html>
<body>
    <h1 align="center">🚨 Incident Alert</h1>

<p align="center">
  <img src="https://img.shields.io/badge/Docker-Containerized-blue?logo=docker&logoColor=white" />
  <img src="https://img.shields.io/badge/ASP.NET_Core-Web_API-purple?logo=dotnet" />
  <img src="https://img.shields.io/badge/Entity_Framework-Core-green?logo=dotnet" />
  <img src="https://img.shields.io/badge/CSharp-.NET_8-informational?logo=csharp" />
  <img src="https://img.shields.io/badge/PostgreSQL-Used-blue?logo=postgresql" />
  <img src="https://img.shields.io/badge/RabbitMQ-Messaging-orange?logo=rabbitmq" />
</p>
    <p>
        Incident Alert is a comprehensive system for incident reporting and management, composed of four services designed for users, moderators, and machine learning processes.
    </p>

 <h2>🛠️ Services</h2>

   <h3>1. 📣 Incident Alert</h3>
    <ul>
        <li><strong>For General Users:</strong> Allows users to create and view incidents.</li>
        <li><strong>Incident Flow:</strong> When an incident is created, it is sent via RabbitMQ using MassTransit to the Incident Alert Management service.</li>
    </ul>

   <h3>2. 👩‍⚖️ Incident Alert Management</h3>
    <ul>
        <li><strong>For Moderators:</strong> Provides moderators with a view of incident requests and tools for managing incidents (approve, delete, or reject).</li>
    </ul>

   <h3>3. 📊 Incident Alert Statistic</h3>
    <ul>
        <li><strong>For Moderators:</strong> Fetches all incidents from the Incident Alert service and generates statistical insights, including:
            <ul>
                <li>The location with the most incidents (overall, past year, month, and day).</li>
                <li>The location with the most incidents by category (overall, past year, month, and day).</li>
                <li>The number of incidents per category.</li>
            </ul>
        </li>
    </ul>

   <h3>4. 🤖 Incident Alert ML</h3>
    <ul>
        <li><strong>For Machine Learning:</strong> Retrieves all incidents from the Incident Alert service and groups them based on text similarity to identify similar incidents.</li>
    </ul>

   <h2>⚙️ Setup Instructions</h2>
    <p>
        Before deploying, make sure to configure the connection strings for the databases used by Incident Alert and Incident Alert Management in <code>appsettings.json</code>.
    </p>

 <h2>📋 TODO</h2>
    <ul>
        <li>**Remove Unused Endpoints:** Identify and eliminate any unused or redundant API endpoints.</li>
        <li>**Improve Machine Learning Text Grouping:** Enhance the text similarity algorithm for more accurate grouping of incidents in the Incident Alert ML service.</li>
    </ul>

   <h2>🚀 Potential Enhancements</h2>
    <ul>
        <li><strong>Database Caching:</strong> Implement caching for each service to store results of frequently requested data, which would reduce the need for repetitive calculations and improve response times.</li>
        <li><strong>Historical Result Storage:</strong> Enable the Statistic and ML services to store their query results in a database. This would allow the services to check if recent results are already available, avoiding unnecessary reprocessing.</li>
        <li><strong>Service-Wide Caching:</strong> Adding caching mechanisms across all services could reduce load and optimize performance, especially for repeated or similar queries.</li>
    </ul>
</body>
</html>
