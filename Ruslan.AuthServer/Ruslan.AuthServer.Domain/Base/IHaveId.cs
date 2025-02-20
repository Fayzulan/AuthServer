﻿namespace Ruslan.AuthServer.Domain.Base;

/// <summary>
/// Identifier common interface
/// </summary>
public interface IHaveId
{
    /// <summary>
    /// Identifier
    /// </summary>
    Guid Id { get; set; }
}