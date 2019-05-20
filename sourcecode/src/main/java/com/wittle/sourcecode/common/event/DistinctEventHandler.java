package com.wittle.sourcecode.common.event;

import java.lang.reflect.Method;
import java.util.HashMap;
import java.util.LinkedList;
import java.util.Map;

import com.wittle.sourcecode.common.log.CFLoggerFactory;
import com.wittle.sourcecode.common.log.ICFLogger;

@SuppressWarnings("rawtypes")
public class DistinctEventHandler {
	protected final ICFLogger logger = CFLoggerFactory.getLogger(this.getClass());
	public static final String eventPath = "com.coolfish.ironforce2.commons.event.";

	private Map<Class, LinkedList<CallBackListenner>> paramMethodMap = new HashMap<Class, LinkedList<CallBackListenner>>();

	private class CallBackListenner<T extends DistinctEvent, E extends DistinctObj> {
		T event;
		E distinctObj;
		Object owner;
		Method callback;

		@Override
		public boolean equals(Object obj) {
			if (obj == null)
				return false;
			if (!(obj instanceof CallBackListenner)) {
				return false;
			}
			CallBackListenner other = (CallBackListenner) obj;
			if (other.event == null) {
				return false;
			}
			if (event.getClass() != other.event.getClass()) {
				return false;
			}
			if (!distinctObj.equals(other.distinctObj)) {
				return false;
			}
			return true;
		}
	}

	@SuppressWarnings("unchecked")
	public <T extends DistinctEvent, E extends DistinctObj> void RegisterEvent(T event, E distinctObj, Object owner, String callbackName) throws Exception {
		LinkedList<CallBackListenner> list;
		Class type = event.getClass();
		if (!paramMethodMap.containsKey(type)) {
			list = new LinkedList<>();
			paramMethodMap.put(type, list);
		}
		list = paramMethodMap.get(type);
		Method callback = owner.getClass().getMethod(callbackName, type, distinctObj.getClass());
		CallBackListenner cbl = new CallBackListenner();
		cbl.owner = owner;
		cbl.callback = callback;
		cbl.event = event;
		cbl.distinctObj = distinctObj;

		if (!list.contains(cbl)) {
			list.add(cbl);
		}
	}

	public <T extends DistinctEvent> void Send(T param) throws Exception {
		LinkedList<CallBackListenner> list = new LinkedList<>();
		Class<? extends DistinctEvent> type = param.getClass();
		for (Map.Entry<Class, LinkedList<CallBackListenner>> entry : paramMethodMap.entrySet()) {
			if (type.isAssignableFrom(entry.getKey())) {
				list.addAll(entry.getValue());
			}
		}
		if (list.size() > 0) {
			for (CallBackListenner cbl : list) {
				if (cbl.event.isConditionMeet(param))
					cbl.callback.invoke(cbl.owner, param, cbl.distinctObj);
			}
		}
	}

	public <T extends DistinctEvent, E extends DistinctObj> boolean CheckEvent(T event, E distinctObj, String callbackName, Object owner) {
		Class<? extends DistinctEvent> type = event.getClass();
		if (!paramMethodMap.containsKey(type)) {
			return false;
		}
		LinkedList<CallBackListenner> list = paramMethodMap.get(type);
		Method callback = null;
		try {
			callback = owner.getClass().getMethod(callbackName, event.getClass(), distinctObj.getClass());
		} catch (NoSuchMethodException e) {
			e.printStackTrace();
			return false;
		}
		CallBackListenner cbl = new CallBackListenner();
		cbl.event = event;
		cbl.distinctObj = distinctObj;
		cbl.owner = owner;
		cbl.callback = callback;
		if (!list.contains(cbl)) {
			return false;
		}
		return true;
	}

	public <T extends DistinctEvent> boolean CheckEvent(T param) {
		Class<? extends DistinctEvent> type = param.getClass();
		if (!paramMethodMap.containsKey(type)) {
			return false;
		}
		return true;
	}

	public <T extends DistinctEvent, E extends DistinctObj> void UnRegister(Class<T> param, DistinctObj distinctObj) throws Exception {
		if (paramMethodMap.containsKey(param)) {
			LinkedList<CallBackListenner> list = paramMethodMap.get(param);
			for (CallBackListenner cbl : list) {
				if (cbl.distinctObj.equals(distinctObj)) {
					list.remove(cbl);
					break;
				}
			}
		}
	}

	public <T extends DistinctEvent> void UnRegister(Class<T> param) {
		if (paramMethodMap.containsKey(param)) {
			paramMethodMap.remove(param);
		}
	}
}
