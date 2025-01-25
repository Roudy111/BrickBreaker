using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Abstract Factory base class implementing the Factory Method pattern.
/// This design enables flexible product creation and supports future game expansion.
/// 
/// Key benefits:
/// - Allows addition of new product types without modifying existing code
/// - Decouples product creation from product usage
/// - Supports interface-based product variations
/// 
/// Future extensibility:
/// - Products can have independent behaviors and properties
/// 
/// Design Pattern: Factory Method Pattern
/// </summary>
public abstract class Factory : MonoBehaviour
{
    public abstract IProduct GetProduct(Vector3 position, Quaternion rotation);
}