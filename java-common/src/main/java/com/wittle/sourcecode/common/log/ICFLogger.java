package com.wittle.sourcecode.common.log;

public interface ICFLogger {

	public boolean isDebugEnabled();
	public void debug(String msg);
	public void debug(String msg,Object ...objects);
	
	public boolean isInfoEnabled();
	public void info(String msg);
	public void info(String msg,Object ...objects);
	
	public void warn(String msg);
	public void warn(String msg, Object ...objects);
	public void warn(String msg, Throwable tx);
	
	public void error(String msg);
	public void error(String msg,Object ...objects);
	public void error(String msg, Throwable tx);

	public void trace(String msg);
	public void trace(String msg, Throwable tx);
	
}
