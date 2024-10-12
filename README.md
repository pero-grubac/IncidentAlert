# Incident Alert

Incident Alert is a comprehensive system for incident reporting and management, composed of four services designed for users, moderators, and machine learning processes.

## Services

1. **Incident Alert**: 
   - **For General Users**: Allows users to create and view incidents.
   - **Incident Flow**: When an incident is created, it is sent via RabbitMQ using MassTransit to the Incident Alert Management service.

2. **Incident Alert Management**:
   - **For Moderators**: Provides moderators with a view of incident requests and tools for managing incidents (approve, delete, or reject).
   
3. **Incident Alert Statistic**:
   - **For Moderators**: Fetches all incidents from the Incident Alert service and generates statistical insights, including:
     - The location with the most incidents (overall, past year, month, and day).
     - The location with the most incidents by category (overall, past year, month, and day).
     - The number of incidents per category.
   
4. **Incident Alert ML**:
   - **For Machine Learning**: Retrieves all incidents from the Incident Alert service and groups them based on text similarity to identify similar incidents.

## Setup Instructions

Before deploying, make sure to configure the connection strings for the databases used by Incident Alert and Incident Alert Management in `appsettings.json`.


## TODO

- **Remove Unused Endpoints**: Identify and eliminate any unused or redundant API endpoints.
- **Improve Machine Learning Text Grouping**: Enhance the text similarity algorithm for more accurate grouping of incidents in the Incident Alert ML service.

## Potential Enhancements

Although not immediate requirements, these enhancements could add value to the system:

- **Database Caching**: Implement caching for each service to store results of frequently requested data, which would reduce the need for repetitive calculations and improve response times.
- **Historical Result Storage**: Enable the Statistic and ML services to store their query results in a database. This would allow the services to check if recent results are already available, avoiding unnecessary reprocessing.
- **Service-Wide Caching**: Adding caching mechanisms across all services could reduce load and optimize performance, especially for repeated or similar queries.
