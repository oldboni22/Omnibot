# 🍪 Omnibot – one process to rule them all

A modular and extensible framework designed to simplify the development of complex bot ecosystems.
**Omnibot** provides a structured, pipeline-based architecture for message handling, 
heavily inspired by the **ASP.NET Core** programming model, 
allowing developers to focus on business logic rather than low-level string parsing and routing.

---

## 🌟 Core Capabilities

* ### 🏛️ ASP.NET-Like Controller Pattern
    Organize your logic using familiar patterns that feel like building a Web API.
  * **Declarative Controllers:** Organized, class-based handlers that mirror the ASP.NET controller experience.
  * **Automatic Discovery:** Automatically scans your assemblies to find and register command handlers at startup.
  * **Attribute-Driven Routing:** Map methods to specific bot commands using simple attributes, similar to Action Routing.
  * **Contextual Integration:** Controllers are fully integrated with Dependency Injection, supporting Scoped, Transient, or Singleton lifetimes for seamless access to application services.

* ### 🌐 Multi-Tenant & Multi-Platform Support
    Manage multiple bot identities and platforms within a single application instance.
  * **Logical Isolation:** Separate logic for different bot instances (e.g., Support Bot vs. Sales Bot) even if they run on the same platform.
  * **Flexible Mapping:** Map connector IDs to routing keys, enabling complex routing scenarios and environment-based configurations.

* ### 🧪 Advanced Argument Transformation
    Move away from manual string splitting. Omnibot features a robust system for converting raw input into meaningful data structures.
  * **Explicit Mapping:** Assign specific converters to commands using string identifiers via custom attributes.
  * **Alias Management:** Support multiple names for the same conversion logic, allowing for varied command syntax and reuse of converters.
  * **Resilient Parsing:** Built-in strategies to decide whether a failed argument conversion should halt execution or allow the process to continue (similar to Model State validation).

* ### ⛓️ Extensible Middleware Pipeline
    The entire message lifecycle is governed by a customizable pipeline, identical in concept to the ASP.NET Core Middleware.
  * **Handling Pipes:** Easily insert custom stages into the message flow, such as authorization, logging, or custom data enrichment.
  * **Short-Circuiting:** Modify or halt the execution flow at any stage before it reaches the controller, providing total control over the request lifecycle.