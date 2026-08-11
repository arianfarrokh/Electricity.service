namespace Bills.Application.Common;

/// <summary>
/// Represents the outcome of an application operation.
/// </summary>
/// <typeparam name="T">The result value type.</typeparam>
public sealed class ApplicationResult<T>
{
    private ApplicationResult(bool isSuccess, T? value, string? error)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
    }

    /// <summary>
    /// Gets a value indicating whether the operation succeeded.
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// Gets the result value when successful.
    /// </summary>
    public T? Value { get; }

    /// <summary>
    /// Gets the error message when the operation failed.
    /// </summary>
    public string? Error { get; }

    /// <summary>
    /// Creates a successful result.
    /// </summary>
    /// <param name="value">The result value.</param>
    /// <returns>A successful <see cref="ApplicationResult{T}"/>.</returns>
    public static ApplicationResult<T> Success(T value) => new(true, value, null);

    /// <summary>
    /// Creates a failed result.
    /// </summary>
    /// <param name="error">The error message.</param>
    /// <returns>A failed <see cref="ApplicationResult{T}"/>.</returns>
    public static ApplicationResult<T> Failure(string error) => new(false, default, error);
}
