package com.wittle.sourcecode.common.event;

public abstract class DistinctEvent {
	protected abstract Boolean isConditionMeet(DistinctEvent other);
}
