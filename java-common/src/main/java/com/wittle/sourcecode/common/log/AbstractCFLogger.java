package com.wittle.sourcecode.common.log;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

public abstract class AbstractCFLogger implements ICFLogger {

	protected final Logger logger;

	protected AbstractCFLogger(Class<?> loggerClass) {
		this.logger = LoggerFactory.getLogger(loggerClass);
	}

	protected AbstractCFLogger(String name) {
		this.logger = LoggerFactory.getLogger(name);
	}

	@Override
	public boolean isDebugEnabled() {
		return logger.isDebugEnabled();
	}

	@Override
	public void debug(String msg) {
		if (!isDebugEnabled()) {
			return;
		} else {
			this.logger.debug(msg);
		}
	}

	@Override
	public void debug(String msg, Object... objects) {
		if (!isDebugEnabled()) {
			return;
		} else {
			this.logger.debug(msg, objects);
		}
	}

	@Override
	public boolean isInfoEnabled() {
		return logger.isInfoEnabled();
	}

	@Override
	public void info(String msg) {
		if (!isInfoEnabled()) {
			return;
		} else {
			logger.info(msg);
		}
	}

	@Override
	public void info(String msg, Object... objects) {
		if (!isInfoEnabled()) {
			return;
		} else {
			logger.info(msg, objects);
		}
	}

	@Override
	public void warn(String msg) {
		logger.warn(msg);
	}

	@Override
	public void warn(String msg, Throwable tx) {
		logger.warn(msg, tx);
	}

	@Override
	public void error(String msg) {
		logger.error(msg);
	}

	@Override
	public void error(String msg, Throwable tx) {
		logger.error(msg, tx);
	}

	@Override
	public void trace(String msg) {
		logger.trace(msg);
	}

	@Override
	public void trace(String msg, Throwable tx) {
		logger.trace(msg, tx);
	}

	@Override
	public void warn(String msg, Object... objects) {
		logger.warn(msg, objects);
	}

	@Override
	public void error(String msg, Object... objects) {
		logger.error(msg, objects);
	}

}
