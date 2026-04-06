using RemoteServer.Services.Windows.KeyboardInput.TextStrategies;
using Xunit;

namespace RemoteServer.Tests;

public class SendKeysTextStrategyTests
{
    [Theory]
    [InlineData('/', "/")]
    [InlineData('+', "{+}")]
    [InlineData('^', "{^}")]
    [InlineData('~', "{~}")]
    [InlineData('(', "{(}")]
    [InlineData(')', "{)}")]
    [InlineData('{', "{{")]
    [InlineData('}', "{}}")]
    public void EscapeForSendKeys_ReturnsCorrectEscape(char input, string expected)
    {
        var result = SendKeysTextStrategy.EscapeForSendKeys(input.ToString());
        Assert.Equal(expected, result);
    }
}
