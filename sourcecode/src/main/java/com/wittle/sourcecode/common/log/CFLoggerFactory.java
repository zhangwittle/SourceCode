package com.wittle.sourcecode.common.log;

import java.io.File;

import org.slf4j.LoggerFactory;

import com.wittle.sourcecode.utils.ConfigPathUtils;

import ch.qos.logback.classic.LoggerContext;
import ch.qos.logback.classic.joran.JoranConfigurator;
import ch.qos.logback.core.joran.spi.JoranException;
import ch.qos.logback.core.util.StatusPrinter;

public class CFLoggerFactory {

	private CFLoggerFactory() {
	}

	public static void init(String logPath) {
		File logbackFile = new File(logPath);
		if (logbackFile.exists()) {
			LoggerContext lc = (LoggerContext) LoggerFactory.getILoggerFactory();
			JoranConfigurator configurator = new JoranConfigurator();
			configurator.setContext(lc);
			lc.reset();
			try {
				configurator.doConfigure(logbackFile);
			} catch (JoranException e) {
				throw new RuntimeException("", e);
			}
			StatusPrinter.printInCaseOfErrorsOrWarnings(lc);
		}
	}

	public static void init() {
		initDefault();
	}

	public static void initDefault() {
		init(ConfigPathUtils.getLogFilePath() + "logback.xml");
	}

	public static ICFLogger getLogger(Class<?> loggerClazz) {
		return new ClazzLogger(loggerClazz);
	}

	public static ICFLogger getLogger(String loggerName) {
		return new NameLogger(loggerName);
	}

}
