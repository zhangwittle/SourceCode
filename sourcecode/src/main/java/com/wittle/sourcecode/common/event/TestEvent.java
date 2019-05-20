package com.wittle.sourcecode.common.event;

public class TestEvent extends CommonEvent {

	@Override
	protected Boolean isConditionMeet(CommonEvent other) {
		return true;
	}

}
