package com.wittle.sourcecode.utils;

import java.io.PrintWriter;
import java.io.StringWriter;

import com.wittle.sourcecode.common.log.CFLoggerFactory;
import com.wittle.sourcecode.common.log.ICFLogger;

public class ExceptionUtils {
	protected ICFLogger logger = CFLoggerFactory.getLogger(this.getClass());
	public static ExceptionUtils instance = new ExceptionUtils();

	public String printStackTraceToString(Throwable t) {
		StringWriter sw = new StringWriter();
		t.printStackTrace(new PrintWriter(sw, true));
		return sw.getBuffer().toString();
	}
}
