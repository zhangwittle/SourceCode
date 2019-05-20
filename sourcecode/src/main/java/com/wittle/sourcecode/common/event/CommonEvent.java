package com.wittle.sourcecode.common.event;

public abstract class CommonEvent {
	protected abstract Boolean isConditionMeet(CommonEvent other);
}
