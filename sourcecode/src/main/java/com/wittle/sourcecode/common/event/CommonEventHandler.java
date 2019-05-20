package com.wittle.sourcecode.common.event;

import java.lang.reflect.Method;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.LinkedList;
import java.util.List;
import java.util.Map;

import org.junit.Test;

import com.wittle.sourcecode.common.log.CFLoggerFactory;
import com.wittle.sourcecode.common.log.ICFLogger;

@SuppressWarnings("rawtypes")
public class CommonEventHandler {
	protected final ICFLogger logger = CFLoggerFactory.getLogger(this.getClass());
	public static final String eventPath = "com.coolfish.ironforce2.commons.event.";

	private Map<Class, LinkedList<CallBackListenner>> paramMethodMap = new HashMap<Class, LinkedList<CallBackListenner>>();
	private Map<String, LinkedList<CallBackListenner>> voidMethodMap = new HashMap<String, LinkedList<CallBackListenner>>();

	private class CallBackListenner<T extends CommonEvent> {
		Object owner;// 不可为空
		Method callback;// 不可为空
		T event;// 可以为空

		@Override
		public boolean equals(Object obj) {
			if (obj == null)
				return false;
			if (!(obj instanceof CallBackListenner)) {
				return false;
			}
			CallBackListenner other = (CallBackListenner) obj;
			if (owner.getClass() != other.owner.getClass() || !other.callback.getName().equals(callback.getName())) {
				return false;
			}
			if (event != null) {
				if (other.event == null) {
					return false;
				} else {
					if (event.getClass() != other.event.getClass()) {
						return false;
					}
				}
			} else {
				if (other.event != null) {
					return false;
				}
			}
			return true;
		}
	}

	@SuppressWarnings("unchecked")
	public <T extends CommonEvent> void RegisterEvent(T event, Object owner, String callbackName) throws Exception {
		LinkedList<CallBackListenner> list;
		Class type = event.getClass();
		if (!paramMethodMap.containsKey(type)) {
			list = new LinkedList<>();
			paramMethodMap.put(type, list);
		}
		list = paramMethodMap.get(type);
		Method callback = owner.getClass().getMethod(callbackName, type);
		CallBackListenner cbl = new CallBackListenner();
		cbl.owner = owner;
		cbl.callback = callback;
		cbl.event = event;
		if (!list.contains(cbl)) {
			list.add(cbl);
		}
	}

	public <T extends CommonEvent> void Send(T param) throws Exception {
		LinkedList<CallBackListenner> list = new LinkedList<>();
		Class<? extends CommonEvent> type = param.getClass();
		for (Map.Entry<Class, LinkedList<CallBackListenner>> entry : paramMethodMap.entrySet()) {
			if (type.isAssignableFrom(entry.getKey())) {
				list.addAll(entry.getValue());
			}
		}
		if (list.size() > 0) {
			for (CallBackListenner cbl : list) {
				if (cbl.event.isConditionMeet(param))
					cbl.callback.invoke(cbl.owner, param);
			}
		}
	}

	public <T extends CommonEvent> boolean CheckEvent(T param, String callbackName, Object owner) {
		Class<? extends CommonEvent> type = param.getClass();
		if (!paramMethodMap.containsKey(type)) {
			return false;
		}
		LinkedList<CallBackListenner> list = paramMethodMap.get(type);
		Method callback = null;
		try {
			callback = owner.getClass().getMethod(callbackName, param.getClass());
		} catch (NoSuchMethodException e) {
			e.printStackTrace();
			return false;
		}
		CallBackListenner cbl = new CallBackListenner();
		cbl.owner = owner;
		cbl.callback = callback;
		cbl.event = param;
		if (!list.contains(cbl)) {
			return false;
		}
		return true;
	}

	public <T extends CommonEvent> boolean CheckEvent(T param) {
		Class<? extends CommonEvent> type = param.getClass();
		if (!paramMethodMap.containsKey(type)) {
			return false;
		}
		return true;
	}

	public <T extends CommonEvent> void UnRegister(Class<T> param, Object owner, String callbackName) throws Exception {
		if (paramMethodMap.containsKey(param)) {
			LinkedList<CallBackListenner> list = paramMethodMap.get(param);
			for (CallBackListenner cbl : list) {
				Method callback = owner.getClass().getMethod(callbackName, param);
				if (cbl.owner.equals(owner) && cbl.callback.equals(callback)) {
					list.remove(cbl);
					break;
				}
			}
			if (voidMethodMap.get(param) == null || voidMethodMap.get(param).size() == 0) {
				voidMethodMap.remove(param);
			}
		}

	}

	public void UnRegister(String eventType, Object owner, String callbackName) throws Exception {
		if (voidMethodMap.containsKey(eventType)) {
			LinkedList<CallBackListenner> list = voidMethodMap.get(eventType);
			for (CallBackListenner cbl : list) {
				Method callback = owner.getClass().getMethod(callbackName);
				if (cbl.owner.equals(owner) && cbl.callback.equals(callback)) {
					list.remove(cbl);
					break;
				}
			}
			if (voidMethodMap.get(eventType) == null || voidMethodMap.get(eventType).size() == 0) {
				voidMethodMap.remove(eventType);
			}
		}

	}

	public void UnRegister(Object owner) throws Exception {
		List<String> needClearMap = new ArrayList<>();
		for (Map.Entry<String, LinkedList<CallBackListenner>> entry : voidMethodMap.entrySet()) {
			LinkedList<CallBackListenner> list = entry.getValue();
			LinkedList<CallBackListenner> newList = new LinkedList<>(list);
			for (CallBackListenner cbl : newList) {
				if (cbl.owner.equals(owner)) {
					list.remove(cbl);
				}
			}
			if (list == null || list.size() == 0) {
				needClearMap.add(entry.getKey());
			}
		}
		for (String clear : needClearMap) {
			voidMethodMap.remove(clear);
		}

	}

	public <T extends CommonEvent> void UnRegister(Class<T> param) {
		if (paramMethodMap.containsKey(param)) {
			paramMethodMap.remove(param);
		}
	}

	public void UnRegister(String eventType) {
		if (voidMethodMap.containsKey(eventType)) {
			voidMethodMap.remove(eventType);
		}
	}

	@Test
	public void Test() throws Exception {
		// RegisterEvent(null, this, "Test4");
		RegisterEvent(new TestEvent(), (Object) this, "Test3");
		boolean result = CheckEvent(new TestEvent(), "Test3", (Object) this);
		System.out.println(result);
	}

	public void Test2(TestEvent test) throws Exception {
		System.out.println("haha");
		// UnRegister(TestEvent.class, this, "Test2");
	}

	public void Test3(TestEvent test) {
		System.out.println("yahaha");
	}

	public void Test4() {
		System.out.println("yayyayayhaha");
	}
}
