using UnityEngine;

/// <summary>
/// Base interface for all factory-produced products in the game.
/// Defines the common contract that all products must fulfill.
/// 
/// Key features:
/// - Provides a common interface for all factory products
/// - Allows products to implement additional interfaces independently
/// - Enables polymorphic handling of different product types
/// 
/// Extensibility:
/// - Products can implement additional interfaces (e.g., IDestructible, IScoreable)
/// - New product properties can be added through interface inheritance
/// - Supports component-based design through interface composition
/// </summary>
public interface IProduct
{

    string ProductName { get; set; }


    // Each product type can implement custom initialization logic
    void Initialize();
}
