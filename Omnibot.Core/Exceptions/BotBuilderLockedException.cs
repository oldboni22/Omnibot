namespace Omnibot.Core.Exceptions;

public sealed class BotBuilderLockedException() : Exception("The bot has already been built and the builder is locked.");
