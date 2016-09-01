using System;
using System.IO;
using System.Text;
using ChMonitoring.Helpers;
using ChMonitoring.MonitData;
using ChMonitoring.Properties;
using NHttp;

namespace ChMonitoring.Http
{
    /// <summary>
    ///     TODO
    /// </summary>
    internal class Cervlet
    {
        #region Fields

        public const string FAVICON_ICO =
            "AAABAAIAEBAAAAAAIABoBAAAJgAAACAgAAAAACAAqBAAAI4EAAAoAAAAEAAAACAAAAABACAAAAAAAAAIAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAABAAAAAgAAAAMAAAADAAAAAgAAAAEAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAEAAAAFAAAADAAAABMAAAAXAAAAFwAAABMAAAAMAAAABQAAAAEAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAEAAAAFAAAAFAAAACgAAAA3AAAAPwAAAD8AAAA3AAAAKAAAABQAAAAFAAAAAQAAAAAAAAAAAAAAAAAAAAAAAAAEAAAAEwAAAC8AjlOHALVkxBzVe+4r3H/uALdoxACOU4cAAAAvAAAAEwAAAAQAAAAAAAAAAAAAAAAAAAABAAAACwAAACUAhUqfAMV6/0j0t/90/9j/hP/f/1b3vv8AyH3/AIdLnwAAACUAAAALAAAAAQAAAAAAAAAAAAAAAgAAABAAbC94AKNY/wDllP8A6Z3/GvCw/yX0t/8A66X/AOaV/wCjWP8AbC94AAAAEAAAAAIAAAAAAAAAAAAAAAIAAAASAGslugCsSv8A1Xr/ANqF/wDbi/8A3ZD/AN+S/wDWfv8Aq0j/AGslugAAABIAAAACAAAAAAAAAAAAAAACAAAAEABmHuoAqzv/Esdp/xTNdv8Uz33/FNKB/xTSgP8Sx2r/AKs7/wBmHuoAAAAQAAAAAgAAAAAAAAAAAAAAAgAAAAsAXxbpDq1N/yC8Yf81xXT/Ncl5/zXJev81xXT/ILxe/w6tTf8AXxbpAAAACwAAAAIAAAAAAAAAAAAAAAEAAAAGAFYQsSOcUP9hzYj/etid/2TPjP9kz4z/etid/2HNiP8jnFD/AFYQsQAAAAYAAAABAAAAAAAAAAAAAAAAAAAAAgBaC2ABcyT/cMyM/67pwf/Q+N3/0Pjd/67pwf9wzIz/AXMk/wBaC2AAAAACAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAACAE4CggJuIP9ns33/iM6a/4jOmv9ns33/Am4g/wBOAoIAAAACAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAEAUgBdAEkArABFAOcARQDnAEkArABSAF0AAAABAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAP//AAD//wAA//8AAPgfAADwDwAA8A8AAOAHAADgBwAA4AcAAOAHAADwDwAA8A8AAPw/AAD//wAA//8AAP//AAAoAAAAIAAAAEAAAAABACAAAAAAAAAgAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAABAAAAAQAAAAIAAAACAAAAAwAAAAMAAAACAAAAAgAAAAEAAAABAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAEAAAABAAAAAwAAAAQAAAAFAAAABgAAAAcAAAAIAAAACAAAAAcAAAAGAAAABQAAAAQAAAADAAAAAQAAAAEAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAABAAAAAwAAAAUAAAAHAAAACgAAAA0AAAAPAAAAEQAAABIAAAASAAAAEQAAAA8AAAANAAAACgAAAAcAAAAFAAAAAwAAAAEAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAQAAAAMAAAAHAAAACwAAABAAAAAVAAAAGQAAAB0AAAAfAAAAIQAAACEAAAAfAAAAHQAAABkAAAAVAAAAEAAAAAsAAAAHAAAAAwAAAAEAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAEAAAADAAAABwAAAA0AAAAUAAAAHAAAACQAAAAqAAAALwAAADMAAAA1AAAANQAAADMAAAAvAAAAKgAAACQAAAAcAAAAFAAAAA0AAAAHAAAAAwAAAAEAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAABAAAAAwAAAAYAAAANAAAAFgAcDiQAQiY2AFAvRgBcNFMDZzleCnJAZxF3RWsVekVrEXRDZwVqPF4AXzRTAFAvRgBCJjYAHA4kAAAAFgAAAA0AAAAGAAAAAwAAAAEAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAIAAAAGAAAADAAAABUAAAAiAD4jOgBzQl0AjFB8AJ5YlAatYqkTvWy6HcZywiPJc8IewnC6C7JlqQCgW5QAjFB8AHNCXQA+IzoAAAAiAAAAFQAAAAwAAAAGAAAAAgAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAABAAAABAAAAAoAAAATACMOJAA+IT4Abj9hAJZZjwasabERv3bIHc6E2yzZjus24ZTzPuOX8zjfk+sk0ojbFMJ8yAevbLEAl1uPAG4/YQA+IT4AIw4kAAAAEwAAAAoAAAAEAAAAAQAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAIAAAAHAAAADwAAABoAQyQ5AG88agCRVJsAsWvKDseB5Svcmu1D6q3zVfK6+WP2xPtr98f7ZPTB+U/ts/M035/tEsuE5QCzbsoAklabAHE+agBDJDkAAAAaAAAADwAAAAcAAAACAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAABAAAAAkAIQsXADIWLgBkNFkAhkiYAKJeyQDBdu0N1o3/KOml/0D0uP9T+cf/YfzQ/2j81P9h+87/S/a//zDrq/8Q2ZD/AMR47QClX8kAiEqYAGQ0WQAyFi4AIQsXAAAACQAAAAQAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAEAAAAFAAAACwBCHSMAWydOAHs8hACWUMYAr2TtAM+A+QThk/8N6J7/Ge6p/ynytf8z9bz/Ofa//zP1vP8f8bH/EOuk/wXjlv8A0IL5AK9l7QCWUMYAezyEAFsnTgBCHSMAAAALAAAABQAAAAEAAAAAAAAAAAAAAAAAAAAAAAAAAQAAAAUAAAAMAE4cLgBnKWoAgTqmAJtO4gC0Y/8A0n//AOKQ/wDllf8E55v/Duqj/xXsqf8Z7qz/FO2q/wbqpP8A55z/AOSU/wDTgP8AtGP/AJtO4gCBOqYAZylqAE4cLgAAAAwAAAAFAAAAAQAAAAAAAAAAAAAAAAAAAAAAAAABAAAABQAAAA0ATxw3AGclgwCAM78AnUXqALZa/wDNdP8A24P/AN2I/wHfjf8E4JL/B+GW/wjimP8G45n/AuOY/wDgkv8A3Ij/AM52/wC1Wv8AnUTqAIAzvwBnJYMATxw3AAAADQAAAAUAAAABAAAAAAAAAAAAAAAAAAAAAAAAAAEAAAAFAAAADQBSGT4AZiGZAHwt1QCdPvEBtVL/A8hq/wTTef8E1n7/BdeD/wXYhv8F2Yn/BdqL/wXbjf8F3I3/BNqI/wTVfv8DyWz/AbVS/wCdPfEAfCzVAGYhmQBSGT4AAAANAAAABQAAAAEAAAAAAAAAAAAAAAAAAAAAAAAAAQAAAAUAAAAMAFIWRABkHqsAeSjnAJw49wOzSv8Kw2H/Dcxw/w7Pd/8P0Xv/D9J//w/Tgv8P1IT/D9WF/w/Vhf8O03//Dc50/wrDY/8Ds0r/AJw39wB5KOcAZB6rAFIWRAAAAAwAAAAFAAAAAQAAAAAAAAAAAAAAAAAAAAAAAAABAAAABQAAAAsAVRZFAGIbswB3Je8Cmzf6CLJJ/xG+Xf8Xxmv/Gspy/xzMd/8czXr/HM59/xzPfv8c0H//HM9+/xrMd/8Xx2z/Eb5d/wiySf8Cmzf6AHcl7wBiG7MAVRZFAAAACwAAAAUAAAABAAAAAAAAAAAAAAAAAAAAAAAAAAEAAAAEAAAACQBUE0MAXheyAnUk7webPfoPsU//GLpc/yDBZ/8oxXD/LMh2/yzKef8sy3r/LMt7/yzLe/8syXj/KMZx/yDBZv8Yulv/D7FO/webPfoCdSTvAF4XsgBUE0MAAAAJAAAABAAAAAEAAAAAAAAAAAAAAAAAAAAAAAAAAQAAAAMAAAAHAFIQPgBbFKcEciTkDphA9hqvVf8pu2P/NcNw/0DIef9Fyn7/Qst+/0DLfv9Ay37/Qst+/0XKfv9AyHn/NcNu/ym7Yv8ar1T/DphA9gRyJOQAWxSnAFIQPgAAAAcAAAADAAAAAQAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAgAAAAUATQ41AFgQkQlvJM8XkkLvKqtb/0S/cv9XzIP/Y9GO/2TSkP9cz4r/WM6H/1jOh/9cz4r/ZNKQ/2PRjf9XzIP/RL9y/yqrW/8XkkLvCW8kzwBYEJEATQ41AAAABQAAAAIAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAABAAAAAwBNDCsAVg13CGsitRWHO+ctoVb/Ur54/23RkP9+2Z//hdyl/4Haov9/2qD/f9qg/4Haov+F3KX/ftmf/23RkP9Svnj/LaFW/xWHO+cIayK1AFYNdwBNDCsAAAADAAAAAQAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAACAFIIHwBXDFgDZxuXCHgp3SKRRv9TuXT/edOW/5Tfrf+m57z/sOzF/7Xuyf+17sn/sOzF/6bnvP+U363/edOW/1O5dP8ikUb/CHgp3QNnG5cAVwxYAFIIHwAAAAIAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAEAUQATAFoJNgBkFW4Aax26F4A16EGkXfhmvoH/itKg/6Xgt/+26cX/vu7N/77uzf+26cX/peC3/4rSoP9mvoH/QaRd+BeANegAax26AGQVbgBaCTYAUQATAAAAAQAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAABVAAYAUQATAFsNOwBcDn8Kah65GH4y6DSUUP9isnn/gceV/5LTpP+a2av/mtmr/5LTpP+Bx5X/YrJ5/zSUUP8YfjLoCmoeuQBcDn8AWw07AFEAEwBVAAYAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAEARwAZAE0ASQBaDH8BZxm6F3gv3ECSVOZZpGruY6py9miudvlornb5Y6py9lmkau5AklTmF3gv3AFnGboAWgx/AE0ASQBHABkAAAABAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAABAAAgARwAZAFgJOgBeDmwKZBaUG2gksiJrKcwkaijiJWop7SVqKe0kaijiImspzBtoJLIKZBaUAF4ObABYCToARwAZAEAACAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAARwASAE4ANABPAFQASgByAEcAjABFAKIARQCtAEUArQBFAKIARwCMAEoAcgBPAFQATgA0AEcAEgAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAqAAYASwARAEkAHABKACYASAAuAEIANgBDADkAQwA5AEIANgBIAC4ASgAmAEkAHABLABEAKgAGAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAP/////////////////////////////////wD///wAP//4AB//8AAP/+AAB//gAAf/wAAD/8AAA//AAAP/wAAD/8AAA//AAAP/wAAD/+AAB//gAAf/8AAP//gAH//8AD///gB///+B//////////////////////////////////";

        #endregion

        private string txtFormatting(string input, int length)
        {
            while (input.Length < length)
            {
                input += " ";
            }
            return input;
        }

        #region Public Methods

        public void Start()
        {
            Processor.doGet += doGet;
            Processor.doPost += doPost;
        }

        public void Stop()
        {
            Processor.doGet -= doGet;
            Processor.doPost -= doPost;
        }

        #endregion

        #region Private Methods

        private void printServiceStatus(StringBuilder sb, Service_T s)
        {
            sb.AppendFormat("<span class='{0}-text'>{1}</span>",
                (s.monitor == MonitMonitorStateType.Monitor_Not ||
                 ((int) s.monitor & (int) MonitMonitorStateType.Monitor_Init) != 0)
                    ? "gray"
                    : ((s.error == 0) ? "green" : "red"), Processor.EscapeHTML(getServiceStatus(s)));
        }

        private void doPost(HttpRequest req, HttpResponse res)
        {
            res.ContentType = "text/html";

            var reqUrl = MonitURLCommands.GetURLCommand(req.RawUrl);
            switch (reqUrl)
            {
                case URLCommands.RUN:
                    handleRun(req, res);
                    break;
                case URLCommands.DOACTION:
                    handleDoAction(req, res);
                    break;
                default:
                    handleAction(req, res);
                    break;
            }
        }

        private void doGet(HttpRequest req, HttpResponse res)
        {
            res.ContentType = "text/html";

            var reqUrl = MonitURLCommands.GetURLCommand(req.RawUrl);
            switch (reqUrl)
            {
                case URLCommands.HOME:
                    doHome(req, res);
                    break;
                case URLCommands.RUN:
                    handleRun(req, res);
                    break;
                case URLCommands.TEST:
                    isMonitRunning(req, res);
                    break;
                case URLCommands.VIEWLOG:
                    doViewLog(req, res);
                    break;
                case URLCommands.ABOUT:
                    doAbout(req, res);
                    break;
                case URLCommands.FAVICON:
                    printFavicon(res);
                    break;
                case URLCommands.PING:
                    doPing(req, res);
                    break;
                case URLCommands.GETID:
                    doGetID(req, res);
                    break;
                case URLCommands.STATUS:
                    printStatus(req, res, 1);
                    break;
                case URLCommands.STATUS2:
                    printStatus(req, res, 2);
                    break;
                case URLCommands.DOACTION:
                    handleDoAction(req, res);
                    break;
                default:
                    handleAction(req, res);
                    break;
            }
        }

        #endregion

        #region Helpers

        private void isMonitRunning(HttpRequest req, HttpResponse res)
        {
            Processor.SetStatus(res, Processor.ServerRunning ? HttpStatusCode.SC_OK : HttpStatusCode.SC_GONE);
        }

        private void printFavicon(HttpResponse res)
        {
            res.ContentType = "image/x-icon";
            var favicon = Encoding.UTF8.GetString(Convert.FromBase64String(FAVICON_ICO));
            using (var writer = new StreamWriter(res.OutputStream))
            {
                writer.Write(favicon);
            }
        }

        private void doHome(HttpRequest req, HttpResponse res)
        {
            var centerString = new StringBuilder();
            doHomeSystem(centerString);
            doHomeProcess(centerString);
            doHomeProgram(centerString);
            doHomeFileSystem(centerString);
            doHomeFile(centerString);
            doHomeFIFO(centerString);
            doHomeDirectory(centerString);
            doHomeNet(centerString);
            doHomeHost(centerString);

            var homeHtmlBuilder = new OutputBuilder(Resources.HomeHtml);
            var uptime = "0"; //TODO
            homeHtmlBuilder.ReplaceAllTags(MonitWindowsAgent.Run.system[0].name, uptime, centerString.ToString());

            var htmlBuilder = new OutputBuilder(Resources.HeaderFooterHtml);
            htmlBuilder.ReplaceAllTags(MonitWindowsAgent.Run.system[0].name, MonitWindowsAgent.Run.polltime, "", "",
                Processor.SERVER_VERSION, homeHtmlBuilder.GetResult());

            using (var writer = new StreamWriter(res.OutputStream))
            {
                writer.Write(htmlBuilder.GetResult());
            }
        }

        private void doAbout(HttpRequest req, HttpResponse res)
        {
            var htmlBuilder = new OutputBuilder(Resources.AboutHtml);
            htmlBuilder.ReplaceAllTags(Processor.SERVER_VERSION);

            using (var writer = new StreamWriter(res.OutputStream))
            {
                writer.Write(htmlBuilder.GetResult());
            }
        }

        private void doPing(HttpRequest req, HttpResponse res)
        {
            using (var writer = new StreamWriter(res.OutputStream))
            {
                writer.Write("pong");
            }
        }

        private void doGetID(HttpRequest req, HttpResponse res)
        {
            using (var writer = new StreamWriter(res.OutputStream))
            {
                writer.Write(UniqueWindowsId.GetOrCreateUniqueId());
            }
        }

        private void doRuntime(HttpRequest req, HttpResponse res)
        {
            var run = MonitWindowsAgent.Run;
            var logfile = !string.IsNullOrEmpty(MonitWindowsAgent.Run.logfile)
                ? string.Format("<tr><td>Logfile</td><td>{0}</td></tr>", MonitWindowsAgent.Run.logfile)
                : "";

            //    if (run.eventlist_dir) {
            //        char slots[STRLEN];
            //        if (run.eventlist_slots < 0)
            //                snprintf(slots, STRLEN, "unlimited");
            //        else
            //                snprintf(slots, STRLEN, "%d", run.eventlist_slots);
            //        StringBuffer_append(res.outputbuffer,
            //                            "<tr><td>Event queue</td><td>base directory %s with %d slots</td></tr>",
            //                            run.eventlist_dir, run.eventlist_slots);
            //}
            var mmonitServer = new StringBuilder();
            if (run.mmonits != null)
            {
                mmonitServer.Append("<tr><td>M/Monit server(s)</td><td>");
                foreach (var c in run.mmonits)
                {
                    mmonitServer.AppendFormat("{0} with timeout {1} seconds</td></tr>",
                        c.url.url,
                        c.timeout/1000);
                    //"{0} with timeout {1} seconds{2}{3}{4}{5}</td></tr>",
                    //c.url.url,
                    //c.timeout / 1000,
                    //c.ssl.use_ssl ? " ssl version " : "",
                    //c.ssl.use_ssl ? sslnames[c.ssl.version] : "",
                    //c.ssl.certmd5 ? " server cert md5 sum " : "",
                    //c.ssl.certmd5 ? c.ssl.certmd5 : "");
                    mmonitServer.Append("<tr><td>&nbsp;</td><td>");
                }
                //printf("\n");
            }

            var mailServers = new StringBuilder();
            if (run.mailservers != null)
            {
                mailServers.Append("<tr><td>Mail server(s)</td><td>");
                foreach (var mta in run.mailservers)
                    mailServers.AppendFormat("{0}:{1}{2}&nbsp;", mta.host, mta.port, "");
                //mta.ssl.use_ssl ? "(ssl)" : "");
                mailServers.Append("</td></tr>");
            }

            var mailFormat = new StringBuilder();
            if (!string.IsNullOrEmpty(run.MailFormat.from))
                mailFormat.AppendFormat("<tr><td>Default mail from</td><td>{0}</td></tr>", run.MailFormat.from);
            if (!string.IsNullOrEmpty(run.MailFormat.subject))
                mailFormat.AppendFormat("<tr><td>Default mail subject</td><td>{0}</td></tr>", run.MailFormat.subject);
            if (!string.IsNullOrEmpty(run.MailFormat.message))
                mailFormat.AppendFormat("<tr><td>Default mail message</td><td>{0}</td></tr>", run.MailFormat.message);

            var httpd = new StringBuilder();
            //if(run.httpd.flags & Httpd_Net) {
            httpd.AppendFormat("<tr><td>httpd bind address</td><td>{0}</td></tr>", /*run.httpd.socket.net.address*/
                !string.IsNullOrEmpty(run.httpd.address) ? run.httpd.address : "Any/All");
            httpd.AppendFormat("<tr><td>httpd portnumber</td><td>{0}</td></tr>", /*run.httpd.socket.net.port*/
                run.httpd.port);
            //} else if(run.httpd.flags & Httpd_Unix) {
            //    StringBuffer_append(res.outputbuffer,
            //                        "<tr><td>httpd unix socket</td><td>%s</td></tr>",
            //                        run.httpd.socket.unix.path);
            //}
            httpd.AppendFormat("<tr><td>httpd signature</td><td>{0}</td></tr>",
                /*run.httpd.flags & Httpd_Signature ? "True" : */"False");
            httpd.AppendFormat("<tr><td>Use ssl encryption</td><td>{0}</td></tr>",
                /*run.httpd.flags & Httpd_Ssl ? "True" : "False"*/run.httpd.ssl);
            //    if (run.httpd.flags & Httpd_Ssl) {
            //        httpd.AppendFormat("<tr><td>PEM key/certificate file</td><td>%s</td></tr>",
            //                            run.httpd.socket.net.ssl.pem);
            //        if (run.httpd.socket.net.ssl.clientpem != null) {
            //                httpd.AppendFormat("<tr><td>Client PEM key/certification"
            //                                    "</td><td>%s</td></tr>", "Enabled");
            //                httpd.AppendFormat("<tr><td>Client PEM key/certificate file"
            //                                    "</td><td>%s</td></tr>", run.httpd.socket.net.ssl.clientpem);
            //        } else {
            //                httpd.AppendFormat("<tr><td>Client PEM key/certification"
            //                                    "</td><td>%s</td></tr>", "Disabled");
            //        }
            //        httpd.AppendFormat("<tr><td>Allow self certified certificates "
            //                            "</td><td>%s</td></tr>", run.httpd.flags & Httpd_AllowSelfSignedCertificates ? "True" : "False");
            //}
            httpd.AppendFormat("<tr><td>httpd auth. style</td><td>{0}</td></tr>",
                /*run.httpd.credentials != null && Engine_hasHostsAllow() ? "Basic Authentication and Host/Net allow list" : run.httpd.credentials ? */
                "Basic Authentication" /* : Engine_hasHostsAllow() ? "Host/Net allow list" : "No authentication"*/);

            //print_alerts(res, Run.maillist);

            var buttons = new StringBuilder();
            //if (! is_readonly(req)) {
            buttons.Append("<table id='buttons'><tr>");
            buttons.Append(
                "<td style='color:red;'><form method=POST action='_runtime'>Stop Monit http server?<input type=hidden name='action' value='stop'><input type=submit value='Go'></form></td>");
            buttons.Append(
                "<td><form method=POST action='_runtime'>Force validate now? <input type=hidden name='action' value='validate'><input type=submit value='Go'></form></td>");

            if (run.dolog && !run.use_syslog)
            {
                buttons.Append(
                    "<td><form method=GET action='_viewlog'>View Monit logfile? <input type=submit value='Go'></form></td>");
            }
            buttons.Append("</tr></table>");
            //}

            var runtimeHtmlBuilder = new OutputBuilder(Resources.RuntimeHtml);
            runtimeHtmlBuilder.ReplaceAllTags(run.id, run.system[0].name, "PID", run.Env.user, run.controlfile, logfile,
                run.pidfile, run.statefile, run.debug.ToString(), run.dolog.ToString(), run.use_syslog.ToString(), ""
                /*eventlist_dir*/, mmonitServer.ToString(), mailServers.ToString(), mailFormat.ToString(), run.polltime,
                run.startdelay, httpd.ToString(), "" /*Alerts*/, buttons.ToString());

            var headerFooterHtmlBuilder = new OutputBuilder(Resources.HeaderFooterHtml);
            headerFooterHtmlBuilder.ReplaceAllTags(run.system[0].name, 1000, "_runtime", "Runtime",
                Processor.SERVER_VERSION, runtimeHtmlBuilder.GetResult());

            using (var writer = new StreamWriter(res.OutputStream))
            {
                writer.Write(headerFooterHtmlBuilder.GetResult());
            }
        }

        private void doViewLog(HttpRequest req, HttpResponse res)
        {
            //if(is_readonly(req)) {
            //    Processor.SendError(res, HttpStatusCode.SC_FORBIDDEN, "You do not have sufficent privileges to access this page");
            //    return;
            //}

            var centerString = new StringBuilder();
            if (MonitWindowsAgent.Run.dolog && !MonitWindowsAgent.Run.use_syslog)
            {
                if (!string.IsNullOrEmpty(MonitWindowsAgent.Run.logfile))
                {
                    centerString.Append("<br><p><form><textarea cols=120 rows=30 readonly>");
                    centerString.Append(File.ReadAllText(MonitWindowsAgent.Run.logfile));
                    centerString.Append("</textarea></form>");
                }
                else
                {
                    centerString.Append("No logfile defined");
                }
            }
            else
            {
                centerString.Append("<b>Cannot view logfile:</b><br>");
                if (!MonitWindowsAgent.Run.dolog)
                    centerString.Append("Monit was started without logging");
                else
                    centerString.Append("Monit uses syslog");
            }

            var htmlBuilder = new OutputBuilder(Resources.HeaderFooterHtml);
            htmlBuilder.ReplaceAllTags(MonitWindowsAgent.Run.system[0].name, 100, "_viewlog", "View log",
                Processor.SERVER_VERSION, centerString.ToString());

            using (var writer = new StreamWriter(res.OutputStream))
            {
                writer.Write(htmlBuilder.GetResult());
            }
        }

        private void handleAction(HttpRequest req, HttpResponse res)
        {
            var name = req.RawUrl.TrimStart('/');
            var s = Util.GetService(name);
            if (s == null)
            {
                Processor.SendError(res, HttpStatusCode.SC_NOT_FOUND, "There is no service named \"{0}\"",
                    !string.IsNullOrEmpty(name) ? name : "");
                return;
            }

            var action = Processor.GetParameter(req, "action");
            if (!string.IsNullOrEmpty(action))
            {
                //if(is_readonly(req)) {
                //    send_error(res, SC_FORBIDDEN, "You do not have sufficent privileges to access this page");
                //    return;
                //}
                var doaction = Util.GetAction(action);
                if (doaction == MonitActionType.Action_Ignored)
                {
                    Processor.SendError(res, HttpStatusCode.SC_BAD_REQUEST, "Invalid action \"{0}\"", action);
                    return;
                }
                if (s.doaction != MonitActionType.Action_Ignored)
                {
                    Processor.SendError(res, HttpStatusCode.SC_SERVICE_UNAVAILABLE,
                        "Other action already in progress -- please try again later");
                    return;
                }
                s.doaction = doaction;
                var token = Processor.GetParameter(req, "token");
                if (!string.IsNullOrEmpty(token))
                {
                    s.token = token;
                }
                Logger.Log.InfoFormat("'{0}' {1} on user request", s.name, action);
                MonitWindowsAgent.Run.doaction = true; /* set the global flag */
                //do_wakeupcall();
            }

            doService(req, res, s);
        }

        private void handleDoAction(HttpRequest req, HttpResponse res)
        {
            Service_T s;
            var doaction = MonitActionType.Action_Ignored;
            var action = Processor.GetParameter(req, "action");
            var token = Processor.GetParameter(req, "token");

            if (!string.IsNullOrEmpty(action))
            {
                //if (is_readonly(req)) {
                //        Processor.SendError(res, SC_FORBIDDEN, "You do not have sufficent privileges to access this page");
                //        return;
                //}
                if ((doaction = Util.GetAction(action)) == MonitActionType.Action_Ignored)
                {
                    Processor.SendError(res, HttpStatusCode.SC_BAD_REQUEST, "Invalid action \"{0}\"", action);
                    return;
                }

                string[] serviceParams = Processor.GetParameter(req, "service").Split(',');
                foreach (var param in serviceParams)
                {
                    if (!string.IsNullOrEmpty(param))
                    {
                        s = Util.GetService(param);
                        if (s == null)
                        {
                            Processor.SendError(res, HttpStatusCode.SC_BAD_REQUEST, "There is no service named \"{0}\"",
                                param);
                            return;
                        }
                        if (s.doaction != MonitActionType.Action_Ignored)
                        {
                            Processor.SendError(res, HttpStatusCode.SC_SERVICE_UNAVAILABLE,
                                "Other action already in progress -- please try again later");
                            return;
                        }
                        s.doaction = doaction;
                        Logger.Log.InfoFormat("'{0}' {1} on user request", s.name, action);
                    }
                }
                /* Set token for last service only so we'll get it back after all services were handled */
                if (!string.IsNullOrEmpty(token))
                {
                    Service_T q = null;
                    foreach (var ser in MonitWindowsAgent.servicelist)
                        if (ser.doaction == doaction)
                            q = ser;
                    if (q != null)
                    {
                        q.token = token;
                    }
                }
                MonitWindowsAgent.Run.doaction = true;
                //do_wakeupcall();
            }
        }

        private void handleRun(HttpRequest req, HttpResponse res)
        {
            var action = Processor.GetParameter(req, "action");
            if (!string.IsNullOrEmpty(action))
            {
                //if(is_readonly(req)) {
                //    send_error(res, SC_FORBIDDEN, "You do not have sufficent privileges to access this page");
                //    return;
                //}
                if (action == "validate")
                {
                    Logger.Log.Info("The Monit http server woke up on user request");
                    //do_wakeupcall();
                }
                else if (action == "stop")
                {
                    Logger.Log.Info("The Monit http server stopped on user request");
                    Processor.SendError(res, HttpStatusCode.SC_SERVICE_UNAVAILABLE, "The Monit http server is stopped");
                    Server.Stop();
                    return;
                }
            }
            doRuntime(req, res);
        }

        private void doService(HttpRequest req, HttpResponse res, Service_T s)
        {
            var serviceTypeDescr = new StringBuilder();
            if (s.type == MonitServiceType.Service_Process)
            {
                //serviceTypeDescr.AppendFormat("<tr><td>{0}</td><td>{0}</td></tr>", s.matchlist ? "Match" : "Pid file", s.path);
            }
            else if (s.type == MonitServiceType.Service_Host)
                serviceTypeDescr.AppendFormat("<tr><td>Address</td><td>{0}</td></tr>", s.path);
            else if (s.type == MonitServiceType.Service_Net)
                serviceTypeDescr.AppendFormat("<tr><td>Interface</td><td>{0}</td></tr>", s.path);
            else if (s.type != MonitServiceType.Service_System)
                serviceTypeDescr.AppendFormat("<tr><td>Path</td><td>{0}</td></tr>", s.path);

            var serviceStatus = new StringBuilder();
            printServiceStatus(serviceStatus, s);

            //    for (ServiceGroup_T sg = servicegrouplist; sg; sg = sg.next)
            //        for (ServiceGroupMember_T sgm = sg.members; sgm; sgm = sgm.next)
            //                if (IS(sgm.name, s.name))
            //                        StringBuffer_append(res.outputbuffer, "<tr><td>Group</td><td class='blue-text'>%s</td></tr>", sg.name);

            //    for (Dependant_T d = s.dependantlist; d; d = d.next) {
            //        if (d.dependant != NULL) {
            //                StringBuffer_append(res.outputbuffer,
            //                                    "<tr><td>Depends on service </td><td> <a href=%s> %s </a></td></tr>",
            //                                    d.dependant, d.dependant);
            //        }
            //}

            var checkingCycle = new StringBuilder();
            if (s.every.type != MonitEveryType.Every_Cycle)
            {
                checkingCycle.Append("<tr><td>Check service</td><td>");
                if (s.every.type == MonitEveryType.Every_SkipCycles)
                {
                    var cycle = s.every as CycleEvery_T;
                    checkingCycle.AppendFormat("every {0} cycle", cycle.number);
                }
                else if (s.every.type == MonitEveryType.Every_Cron)
                {
                    var cron = s.every as CronEvery_T;
                    checkingCycle.AppendFormat("every <code>\"{0}\"</code>", cron.cron);
                }
                else if (s.every.type == MonitEveryType.Every_NotInCron)
                {
                    var cron = s.every as CronEvery_T;
                    checkingCycle.AppendFormat("not every <code>\"{0}\"</code>", cron.cron);
                }
                checkingCycle.Append("</td></tr>");
            }

            var serviceData = new StringBuilder();
            // Status
            switch (s.type)
            {
                case MonitServiceType.Service_Filesystem:
                    var filesystem = s.inf as FileSystemInfo_T;
                    print_service_status_perm(serviceData, s, filesystem.mode);
                    //print_service_status_uid(res, s, filesystem.uid);
                    //print_service_status_gid(res, s, filesystem.gid);
                    //print_service_status_filesystem_flags(res, s);
                    print_service_status_filesystem_blockstotal(serviceData, s);
                    print_service_status_filesystem_blocksfree(serviceData, s);
                    print_service_status_filesystem_blocksfreetotal(serviceData, s);
                    print_service_status_filesystem_blocksize(serviceData, s);
                    print_service_status_filesystem_inodestotal(serviceData, s);
                    print_service_status_filesystem_inodesfree(serviceData, s);
                    break;
                //        case MonitServiceType.Service_Directory:
                //            var directory = s.inf as DirectoryInfo_T;
                //            print_service_status_perm(res, s, directory.mode);
                //            print_service_status_uid(res, s, directory.uid);
                //            print_service_status_gid(res, s, directory.gid);
                //            print_service_status_timestamp(res, s, directory.timestamp);
                //            break;
                //        case MonitServiceType.Service_File:
                //            var file = s.inf as FileInfo_T;
                //            print_service_status_perm(res, s, file.mode);
                //            print_service_status_uid(res, s, file.uid);
                //            print_service_status_gid(res, s, file.gid);
                //            print_service_status_timestamp(res, s, file.timestamp);
                //            print_service_status_file_size(res, s);
                //            print_service_status_file_match(res, s);
                //            print_service_status_file_checksum(res, s);
                //            break;
                case MonitServiceType.Service_Process:
                    var process = s.inf as ProcessInfo_T;
                    //print_service_status_process_pid(res, s);
                    //print_service_status_process_ppid(res, s);
                    //print_service_status_uid(res, s, process.uid);
                    //print_service_status_process_euid(res, s);
                    //print_service_status_gid(res, s, gid);
                    print_service_status_process_uptime(serviceData, s);
                    print_service_status_process_children(serviceData, s);
                    print_service_status_process_cpu(serviceData, s);
                    print_service_status_process_cputotal(serviceData, s);
                    print_service_status_process_memory(serviceData, s);
                    print_service_status_process_memorytotal(serviceData, s);
                    //print_service_status_port(res, s);
                    //print_service_status_socket(res, s);
                    break;
                //        case MonitServiceType.Service_Host:
                //            print_service_status_icmp(res, s);
                //            print_service_status_port(res, s);
                //            break;
                case MonitServiceType.Service_System:
                    print_service_status_system_loadavg(serviceData, s);
                    print_service_status_system_cpu(serviceData, s);
                    print_service_status_system_memory(serviceData, s);
                    print_service_status_system_swap(serviceData, s);
                    break;
                //        case MonitServiceType.Service_Fifo:
                //            var fifo = s.inf as FiFoInfo_T;
                //            print_service_status_perm(res, s, fifo.mode);
                //            print_service_status_uid(res, s, fifo.uid);
                //            print_service_status_gid(res, s, fifo.gid);
                //            print_service_status_timestamp(res, s, fifo.timestamp);
                //            break;
                case MonitServiceType.Service_Program:
                    print_service_status_program_started(serviceData, s);
                    print_service_status_program_status(serviceData, s);
                    print_service_status_program_output(serviceData, s);
                    break;
                //        case MonitServiceType.Service_Net:
                //            print_service_status_link(res, s);
                //            print_service_status_download(res, s);
                //            print_service_status_upload(res, s);
                //            break;
                default:
                    break;
            }

            var serviceRules = new StringBuilder();
            //    // Rules
            //    print_service_rules_timeout(res, s);
            //    print_service_rules_existence(res, s);
            //    print_service_rules_icmp(res, s);
            //    print_service_rules_port(res, s);
            //    print_service_rules_socket(res, s);
            //    print_service_rules_perm(res, s);
            //    print_service_rules_uid(res, s);
            //    print_service_rules_euid(res, s);
            //    print_service_rules_gid(res, s);
            //    print_service_rules_timestamp(res, s);
            //    print_service_rules_fsflags(res, s);
            //    print_service_rules_filesystem(res, s);
            //    print_service_rules_size(res, s);
            //    print_service_rules_linkstatus(res, s);
            //    print_service_rules_linkspeed(res, s);
            //    print_service_rules_linksaturation(res, s);
            //    print_service_rules_uploadbytes(res, s);
            //    print_service_rules_uploadpackets(res, s);
            //    print_service_rules_downloadbytes(res, s);
            //    print_service_rules_downloadpackets(res, s);
            print_service_rules_uptime(serviceRules, s);
            //    print_service_rules_match(res, s);
            //    print_service_rules_checksum(res, s);
            //    print_service_rules_pid(res, s);
            //    print_service_rules_ppid(res, s);
            //    print_service_rules_program(res, s);
            print_service_rules_resource(serviceRules, s);
            //    //print_alerts(res, s.maillist);

            var buttons = new StringBuilder();
            printButtons(buttons, s);

            var serviceHtmlBuilder = new OutputBuilder(Resources.ServiceHtml);
            serviceHtmlBuilder.ReplaceAllTags(MonitWindowsAgent.servicetypes[(int) s.type], s.name,
                serviceTypeDescr.ToString(), serviceStatus.ToString(), "ServiceGroup",
                MonitWindowsAgent.modenames[(int) s.mode], getMonitoringStatus(s), "Dependant",
                printAction("Start", s.start), printAction("Stop", s.stop), printAction("Restart", s.restart),
                checkingCycle.ToString(), serviceData.ToString(), Timing.GetTimestamp(s.collected),
                serviceRules.ToString(), /*"{ALERTS}"*/"", buttons.ToString());

            var headerFooterHtmlBuilder = new OutputBuilder(Resources.HeaderFooterHtml);
            headerFooterHtmlBuilder.ReplaceAllTags(MonitWindowsAgent.Run.system[0].name, MonitWindowsAgent.Run.polltime,
                s.name, s.name, Processor.SERVER_VERSION, serviceHtmlBuilder.GetResult());

            using (var writer = new StreamWriter(res.OutputStream))
            {
                writer.Write(headerFooterHtmlBuilder.GetResult());
            }
        }

        private static string printAction(string title, Func<bool> s)
        {
            var result = new StringBuilder();
            if (s != null)
            {
                var i = 0;
                result.AppendFormat("<tr><td>{0} program</td><td>'", title);
                result.Append(s.Method.Name);
                //while(s.arg[i]) {
                //    if(i)
                //        result.Append(" ");
                //    result.Append(s.arg[i++]);
                //}
                result.Append("'");
                //if(s.has_uid)
                //    result.AppendFormat(" as uid {0}", s.uid);
                //if(s.has_gid)
                //    result.AppendFormat(" as gid {0}", s.gid);
                result.AppendFormat(" timeout {0} second(s)", /*TODO*/5);
                result.Append("</td></tr>");
            }
            return result.ToString();
        }

        //if(s.stop) {
        //        int i = 0;
        //        StringBuffer_append(res.outputbuffer, "<tr><td>Stop program</td><td>'");
        //        while(s.stop.arg[i]) {
        //            if(i)
        //                StringBuffer_append(res.outputbuffer, " ");
        //            StringBuffer_append(res.outputbuffer, "{0}", s.stop.arg[i++]);
        //        }
        //        StringBuffer_append(res.outputbuffer, "'");
        //        if(s.stop.has_uid)
        //            StringBuffer_append(res.outputbuffer, " as uid {0}", s.stop.uid);
        //        if(s.stop.has_gid)
        //            StringBuffer_append(res.outputbuffer, " as gid {0}", s.stop.gid);
        //        StringBuffer_append(res.outputbuffer, " timeout {0} second(s)", s.stop.timeout);
        //        StringBuffer_append(res.outputbuffer, "</td></tr>");
        //    }

        //    if(s.restart) {
        //        int i = 0;
        //        StringBuffer_append(res.outputbuffer, "<tr><td>Restart program</td><td>'");
        //        while(s.restart.arg[i]) {
        //            if(i)
        //                StringBuffer_append(res.outputbuffer, " ");
        //            StringBuffer_append(res.outputbuffer, "{0}", s.restart.arg[i++]);
        //        }
        //        StringBuffer_append(res.outputbuffer, "'");
        //        if(s.restart.has_uid)
        //            StringBuffer_append(res.outputbuffer, " as uid {0}", s.restart.uid);
        //        if(s.restart.has_gid)
        //            StringBuffer_append(res.outputbuffer, " as gid {0}", s.restart.gid);
        //        StringBuffer_append(res.outputbuffer, " timeout {0} second(s)", s.restart.timeout);
        //        StringBuffer_append(res.outputbuffer, "</td></tr>");
        //    }

        private void doHomeSystem(StringBuilder res)
        {
            var s = MonitWindowsAgent.Run.system[0];
            var systeminfo = MonitWindowsAgent.systeminfo;

            res.Append(
                "<table id='header-row'><tr><th align='left' class='first'>System</th><th align='left'>Status</th>");

            if (MonitWindowsAgent.Run.doprocess)
            {
                res.Append(
                    "<th align='right'>Load</th><th align='right'>CPU</th><th align='right'>Memory</th><th align='right'>Swap</th>");
            }
            res.AppendFormat("</tr><tr class='stripe'><td align='left'><a href='{0}'>{1}</a></td><td align='left'>",
                s.name, s.name);
            printServiceStatus(res, s);
            res.Append("</td>");
            if (MonitWindowsAgent.Run.doprocess)
            {
                res.AppendFormat(
                    "<td align='right'>[{0}]&nbsp;[{1}]&nbsp;[{2}]</td><td align='right'>{3}%us,&nbsp;{4}%sy</td>",
                    systeminfo.loadavg[0], systeminfo.loadavg[1], systeminfo.loadavg[2],
                    systeminfo.total_cpu_user_percent > 0 ? systeminfo.total_cpu_user_percent : 0,
                    systeminfo.total_cpu_syst_percent > 0 ? systeminfo.total_cpu_syst_percent : 0
                    //#ifdef HAVE_CPU_WAIT
                    //                                    ",&nbsp;%.1f%%wa"
                    //#endif
                    //#ifdef HAVE_CPU_WAIT
                    //                                    , systeminfo.total_cpu_wait_percent > 0 ? systeminfo.total_cpu_wait_percent/10. : 0
                    //#endif
                    );
                res.AppendFormat("<td align='right'>{0}% [{1}]</td>", systeminfo.total_mem_percent,
                    systeminfo.total_mem_kbyte*1024);
                res.AppendFormat("<td align='right'>{0}% [{1}]</td>", systeminfo.total_swap_percent,
                    systeminfo.total_swap_kbyte*1024);
            }
            res.Append("</tr></table>");
        }

        private void doHomeProcess(StringBuilder res)
        {
            var on = true;
            var header = true;

            foreach (var s in MonitWindowsAgent.servicelist_conf)
            {
                if (s.type != MonitServiceType.Service_Process)
                    continue;
                var process = s.inf as ProcessInfo_T;
                if (header)
                {
                    res.Append(
                        "<table id='header-row'><tr><th align='left' class='first'>Process</th><th align='left'>Status</th><th align='right'>Uptime</th>");
                    if (MonitWindowsAgent.Run.doprocess)
                    {
                        res.Append("<th align='right'>CPU Total</b></th><th align='right'>Memory Total</th>");
                    }
                    res.Append("</tr>");
                    header = false;
                }
                res.AppendFormat("<tr {0}><td align='left'><a href='{1}'>{2}</a></td><td align='left'>",
                    on ? "class='stripe'" : "", s.name, s.name);
                printServiceStatus(res, s);
                res.Append("</td>");
                if (!Util.HasServiceStatus(s))
                {
                    res.Append("<td align='right'>-</td>");
                    if (MonitWindowsAgent.Run.doprocess)
                    {
                        res.Append("<td align='right'>-</td><td align='right'>-</td>");
                    }
                }
                else
                {
                    //char *uptime = Util_getUptime(s.inf.priv.process.uptime, "&nbsp;");
                    res.AppendFormat("<td align='right'>{0}</td>", /*uptime*/5);
                    if (MonitWindowsAgent.Run.doprocess)
                    {
                        res.AppendFormat("<td align='right' class='{0}'>{1}%</td>",
                            (s.error & (int) MonitEventType.Event_Resource) != 0 ? "red-text" : "",
                            process.total_cpu_percent);
                        res.AppendFormat("<td align='right' class='{0}'>{1}% [{2}]</td>",
                            (s.error & (int) MonitEventType.Event_Resource) != 0 ? "red-text" : "",
                            process.total_mem_percent, process.total_mem_kbyte*1024);
                    }
                }
                res.Append("</tr>");
                on = !on;
            }
            if (!header)
                res.Append("</table>");
        }

        private void doHomeProgram(StringBuilder res)
        {
        }

        private void doHomeNet(StringBuilder res)
        {
        }

        private void doHomeFileSystem(StringBuilder res)
        {
            var on = true;
            var header = true;

            foreach (var s in MonitWindowsAgent.servicelist_conf)
            {
                if (s.type != MonitServiceType.Service_Filesystem)
                    continue;
                var filesystem = s.inf as FileSystemInfo_T;
                if (header)
                {
                    res.Append(
                        "<table id='header-row'><tr><th align='left' class='first'>Filesystem</th><th align='left'>Status</th><th align='right'>Space usage</th><th align='right'>Inodes usage</th></tr>");
                    header = false;
                }
                res.AppendFormat("<tr {0}><td align='left'><a href='{1}'>{2}</a></td><td align='left'>",
                    on ? "class='stripe'" : "", s.name, s.name);
                printServiceStatus(res, s);
                res.Append("</td>");
                if (!Util.HasServiceStatus(s))
                {
                    res.Append("<td align='right'>- [-]</td><td align='right'>- [-]</td>");
                }
                else
                {
                    res.AppendFormat("<td align='right'>{0}% [{1}]</td>", filesystem.space_percent,
                        filesystem.f_bsize > 0 ? (filesystem.space_total*filesystem.f_bsize).ToString() : "0 MB");
                    if (filesystem.f_files > 0)
                    {
                        res.AppendFormat("<td align='right'>{0}% [{1} objects]</td>",
                            filesystem.inode_percent,
                            filesystem.inode_total);
                    }
                    else
                    {
                        res.Append("<td align='right'>not supported by filesystem</td>");
                    }
                }
                res.Append("</tr>");
                on = !on;
            }
            if (!header)
                res.Append("</table>");
        }

        private void doHomeFile(StringBuilder res)
        {
        }

        private void doHomeFIFO(StringBuilder res)
        {
        }

        private void doHomeDirectory(StringBuilder res)
        {
        }

        private void doHomeHost(StringBuilder res)
        {
        }

        #endregion

        #region Unnamed

        //private void printAlerts(StringBuilder res, Mail_T s) {
        //TODO
        //}

        private void printButtons( /*HttpRequest req, */ StringBuilder res, Service_T s)
        {
            //if (is_readonly(req)) {
            //         // A read-only REMOTE_USER does not get access to these buttons
            //        return;
            //}
            res.Append("<table id='buttons'><tr>");
            /* Start program */
            if (s.start != null)
                res.AppendFormat(
                    "<td><form method=POST action={0}><input type=hidden value='start' name=action><input type=submit value='Start service'></form></td>",
                    s.name);
            /* Stop program */
            if (s.stop != null)
                res.AppendFormat(
                    "<td><form method=POST action={0}><input type=hidden value='stop' name=action><input type=submit value='Stop service'></form></td>",
                    s.name);
            /* Restart program */
            if ((s.start != null && s.stop != null) || s.restart != null)
                res.AppendFormat(
                    "<td><form method=POST action={0}><input type=hidden value='restart' name=action><input type=submit value='Restart service'></form></td>",
                    s.name);
            /* (un)monitor */
            res.AppendFormat(
                "<td><form method=POST action={0}><input type=hidden value='{1}' name=action><input type=submit value='{2}'></form></td></tr></table>",
                s.name, s.monitor != MonitMonitorStateType.Monitor_Not ? "unmonitor" : "monitor",
                s.monitor != MonitMonitorStateType.Monitor_Not ? "Disable monitoring" : "Enable monitoring");
        }

//        static void print_service_rules_timeout(HttpResponse res, Service_T s) {
//        for (ActionRate_T ar = s.actionratelist; ar; ar = ar.next) {
//                StringBuffer_append(res.outputbuffer, "<tr class='rule'><td>Timeout</td><td>If restarted %d times within %d cycle(s) then ", ar.count, ar.cycle);
//                Util_printAction(ar.action.failed, res.outputbuffer);
//                StringBuffer_append(res.outputbuffer, "</td></tr>");
//        }
//}


//static void print_service_rules_existence(HttpResponse res, Service_T s) {
//        for (Nonexist_T l = s.nonexistlist; l; l = l.next) {
//                StringBuffer_append(res.outputbuffer, "<tr class='rule'><td>Existence</td><td>");
//                Util_printRule(res.outputbuffer, l.action, "If doesn't exist");
//                StringBuffer_append(res.outputbuffer, "</td></tr>");
//        }
//}


//static void print_service_rules_port(HttpResponse res, Service_T s) {
//        for (Port_T p = s.portlist; p; p = p.next) {
//                StringBuffer_append(res.outputbuffer, "<tr class='rule'><td>Port</td><td>");
//                if (p.retry > 1)
//                        Util_printRule(res.outputbuffer, p.action, "If failed [%s]:%d%s type %s/%s protocol %s with timeout %d seconds and retry %d times", p.hostname, p.port, p.request ? p.request : "", Util_portTypeDescription(p), Util_portIpDescription(p), p.protocol.name, p.timeout / 1000, p.retry);
//                else
//                        Util_printRule(res.outputbuffer, p.action, "If failed [%s]:%d%s type %s/%s protocol %s with timeout %d seconds", p.hostname, p.port, p.request ? p.request : "", Util_portTypeDescription(p), Util_portIpDescription(p), p.protocol.name, p.timeout / 1000);
//                StringBuffer_append(res.outputbuffer, "</td></tr>");
//                if (p.SSL.certmd5 != NULL)
//                        StringBuffer_append(res.outputbuffer, "<tr class='rule'><td>Server certificate md5 sum</td><td>%s</td></tr>", p.SSL.certmd5);
//        }
//}


//static void print_service_rules_socket(HttpResponse res, Service_T s) {
//        for (Port_T p = s.socketlist; p; p = p.next) {
//                StringBuffer_append(res.outputbuffer, "<tr class='rule'><td>Unix Socket</td><td>");
//                if (p.retry > 1)
//                        Util_printRule(res.outputbuffer, p.action, "If failed %s type %s protocol %s with timeout %d seconds and retry %d time(s)", p.pathname, Util_portTypeDescription(p), p.protocol.name, p.timeout / 1000, p.retry);
//                else
//                        Util_printRule(res.outputbuffer, p.action, "If failed %s type %s protocol %s with timeout %d seconds", p.pathname, Util_portTypeDescription(p), p.protocol.name, p.timeout / 1000);
//                StringBuffer_append(res.outputbuffer, "</td></tr>");
//        }
//}


//static void print_service_rules_icmp(HttpResponse res, Service_T s) {
//        for (Icmp_T i = s.icmplist; i; i = i.next) {
//                switch (i.family) {
//                        case Socket_Ip4:
//                                StringBuffer_append(res.outputbuffer, "<tr class='rule'><td>Ping4</td><td>");
//                                break;
//                        case Socket_Ip6:
//                                StringBuffer_append(res.outputbuffer, "<tr class='rule'><td>Ping6</td><td>");
//                                break;
//                        default:
//                                StringBuffer_append(res.outputbuffer, "<tr class='rule'><td>Ping</td><td>");
//                                break;
//                }
//                Util_printRule(res.outputbuffer, i.action, "If failed [count %d with timeout %d seconds]", i.count, i.timeout / 1000);
//                StringBuffer_append(res.outputbuffer, "</td></tr>");
//        }
//}


//static void print_service_rules_perm(HttpResponse res, Service_T s) {
//        if (s.perm) {
//                StringBuffer_append(res.outputbuffer, "<tr class='rule'><td>Permissions</td><td>");
//                if (s.perm.test_changes)
//                        Util_printRule(res.outputbuffer, s.perm.action, "If changed");
//                else
//                        Util_printRule(res.outputbuffer, s.perm.action, "If failed %o", s.perm.perm);
//                StringBuffer_append(res.outputbuffer, "</td></tr>");
//        }
//}


//static void print_service_rules_uid(HttpResponse res, Service_T s) {
//        if (s.uid) {
//                StringBuffer_append(res.outputbuffer, "<tr class='rule'><td>UID</td><td>");
//                Util_printRule(res.outputbuffer, s.uid.action, "If failed %d", s.uid.uid);
//                StringBuffer_append(res.outputbuffer, "</td></tr>");
//        }
//}


//static void print_service_rules_euid(HttpResponse res, Service_T s) {
//        if (s.euid) {
//                StringBuffer_append(res.outputbuffer, "<tr class='rule'><td>EUID</td><td>");
//                Util_printRule(res.outputbuffer, s.euid.action, "If failed %d", s.euid.uid);
//                StringBuffer_append(res.outputbuffer, "</td></tr>");
//        }
//}


//static void print_service_rules_gid(HttpResponse res, Service_T s) {
//        if (s.gid) {
//                StringBuffer_append(res.outputbuffer, "<tr class='rule'><td>GID</td><td>");
//                Util_printRule(res.outputbuffer, s.gid.action, "If failed %d", s.gid.gid);
//                StringBuffer_append(res.outputbuffer, "</td></tr>");
//        }
//}


//static void print_service_rules_timestamp(HttpResponse res, Service_T s) {
//        for (Timestamp_T t = s.timestamplist; t; t = t.next) {
//                StringBuffer_append(res.outputbuffer, "<tr class='rule'><td>Timestamp</td><td>");
//                if (t.test_changes)
//                        Util_printRule(res.outputbuffer, t.action, "If changed");
//                else
//                        Util_printRule(res.outputbuffer, t.action, "If %s %d second(s)", operatornames[t.operator], t.time);
//                StringBuffer_append(res.outputbuffer, "</td></tr>");
//        }
//}


//static void print_service_rules_fsflags(HttpResponse res, Service_T s) {
//        for (Fsflag_T l = s.fsflaglist; l; l = l.next) {
//                StringBuffer_append(res.outputbuffer, "<tr class='rule'><td>Filesystem flags</td><td>");
//                Util_printRule(res.outputbuffer, l.action, "If changed");
//                StringBuffer_append(res.outputbuffer, "</td></tr>");
//        }
//}


//static void print_service_rules_filesystem(HttpResponse res, Service_T s) {
//        for (Filesystem_T dl = s.filesystemlist; dl; dl = dl.next) {
//                if (dl.resource == Resource_Inode) {
//                        StringBuffer_append(res.outputbuffer, "<tr class='rule'><td>Inodes usage limit</td><td>");
//                        if (dl.limit_absolute > -1)
//                                Util_printRule(res.outputbuffer, dl.action, "If %s %lld", operatornames[dl.operator], dl.limit_absolute);
//                        else
//                                Util_printRule(res.outputbuffer, dl.action, "If %s %.1f%%", operatornames[dl.operator], dl.limit_percent / 10.);
//                        StringBuffer_append(res.outputbuffer, "</td></tr>");
//                } else if (dl.resource == Resource_Space) {
//                        StringBuffer_append(res.outputbuffer, "<tr class='rule'><td>Space usage limit</td><td>");
//                        if (dl.limit_absolute > -1) {
//                                if (s.inf.priv.filesystem.f_bsize > 0) {
//                                        char buf[STRLEN];
//                                        Util_printRule(res.outputbuffer, dl.action, "If %s %s", operatornames[dl.operator], Str_bytesToSize(dl.limit_absolute * s.inf.priv.filesystem.f_bsize, buf));
//                                } else {
//                                        Util_printRule(res.outputbuffer, dl.action, "If %s %lld blocks", operatornames[dl.operator], dl.limit_absolute);
//                                }
//                        } else {
//                                Util_printRule(res.outputbuffer, dl.action, "If %s %.1f%%", operatornames[dl.operator], dl.limit_percent / 10.);
//                        }
//                        StringBuffer_append(res.outputbuffer, "</td></tr>");
//                }
//        }
//}


//static void print_service_rules_size(HttpResponse res, Service_T s) {
//        for (Size_T sl = s.sizelist; sl; sl = sl.next) {
//                StringBuffer_append(res.outputbuffer, "<tr class='rule'><td>Size</td><td>");
//                if (sl.test_changes)
//                        Util_printRule(res.outputbuffer, sl.action, "If changed");
//                else
//                        Util_printRule(res.outputbuffer, sl.action, "If %s %llu byte(s)", operatornames[sl.operator], sl.size);
//                StringBuffer_append(res.outputbuffer, "</td></tr>");
//        }
//}


//static void print_service_rules_linkstatus(HttpResponse res, Service_T s) {
//        for (LinkStatus_T l = s.linkstatuslist; l; l = l.next) {
//                StringBuffer_append(res.outputbuffer, "<tr class='rule'><td>Link status</td><td>");
//                Util_printRule(res.outputbuffer, l.action, "If failed");
//                StringBuffer_append(res.outputbuffer, "</td></tr>");
//        }
//}


//static void print_service_rules_linkspeed(HttpResponse res, Service_T s) {
//        for (LinkSpeed_T l = s.linkspeedlist; l; l = l.next) {
//                StringBuffer_append(res.outputbuffer, "<tr class='rule'><td>Link capacity</td><td>");
//                Util_printRule(res.outputbuffer, l.action, "If changed");
//                StringBuffer_append(res.outputbuffer, "</td></tr>");
//        }
//}


//static void print_service_rules_linksaturation(HttpResponse res, Service_T s) {
//        for (LinkSaturation_T l = s.linksaturationlist; l; l = l.next) {
//                StringBuffer_append(res.outputbuffer, "<tr class='rule'><td>Link saturation</td><td>");
//                Util_printRule(res.outputbuffer, l.action, "If %s %.1f%%", operatornames[l.operator], l.limit);
//                StringBuffer_append(res.outputbuffer, "</td></tr>");
//        }
//}


//static void print_service_rules_uploadbytes(HttpResponse res, Service_T s) {
//        char buf[STRLEN];
//        for (Bandwidth_T bl = s.uploadbyteslist; bl; bl = bl.next) {
//                if (bl.range == Time_Second) {
//                        StringBuffer_append(res.outputbuffer, "<tr class='rule'><td>Upload bytes</td><td>");
//                        Util_printRule(res.outputbuffer, bl.action, "If %s %s/s", operatornames[bl.operator], Str_bytesToSize(bl.limit, buf));
//                } else {
//                        StringBuffer_append(res.outputbuffer, "<tr class='rule'><td>Total upload bytes</td><td>");
//                        Util_printRule(res.outputbuffer, bl.action, "If %s %s in last %d %s(s)", operatornames[bl.operator], Str_bytesToSize(bl.limit, buf), bl.rangecount, Util_timestr(bl.range));
//                }
//                StringBuffer_append(res.outputbuffer, "</td></tr>");
//        }
//}


//static void print_service_rules_uploadpackets(HttpResponse res, Service_T s) {
//        for (Bandwidth_T bl = s.uploadpacketslist; bl; bl = bl.next) {
//                if (bl.range == Time_Second) {
//                        StringBuffer_append(res.outputbuffer, "<tr class='rule'><td>Upload packets</td><td>");
//                        Util_printRule(res.outputbuffer, bl.action, "If %s %lld packets/s", operatornames[bl.operator], bl.limit);
//                } else {
//                        StringBuffer_append(res.outputbuffer, "<tr class='rule'><td>Total upload packets</td><td>");
//                        Util_printRule(res.outputbuffer, bl.action, "If %s %lld packets in last %d %s(s)", operatornames[bl.operator], bl.limit, bl.rangecount, Util_timestr(bl.range));
//                }
//                StringBuffer_append(res.outputbuffer, "</td></tr>");
//        }
//}


//static void print_service_rules_downloadbytes(HttpResponse res, Service_T s) {
//        char buf[STRLEN];
//        for (Bandwidth_T bl = s.downloadbyteslist; bl; bl = bl.next) {
//                if (bl.range == Time_Second) {
//                        StringBuffer_append(res.outputbuffer, "<tr class='rule'><td>Download bytes</td><td>");
//                        Util_printRule(res.outputbuffer, bl.action, "If %s %s/s", operatornames[bl.operator], Str_bytesToSize(bl.limit, buf));
//                } else {
//                        StringBuffer_append(res.outputbuffer, "<tr class='rule'><td>Total download bytes</td><td>");
//                        Util_printRule(res.outputbuffer, bl.action, "If %s %s in last %d %s(s)", operatornames[bl.operator], Str_bytesToSize(bl.limit, buf), bl.rangecount, Util_timestr(bl.range));
//                }
//                StringBuffer_append(res.outputbuffer, "</td></tr>");
//        }
//}


//static void print_service_rules_downloadpackets(HttpResponse res, Service_T s) {
//        for (Bandwidth_T bl = s.downloadpacketslist; bl; bl = bl.next) {
//                if (bl.range == Time_Second) {
//                        StringBuffer_append(res.outputbuffer, "<tr class='rule'><td>Download packets</td><td>");
//                        Util_printRule(res.outputbuffer, bl.action, "If %s %lld packets/s", operatornames[bl.operator], bl.limit);
//                } else {
//                        StringBuffer_append(res.outputbuffer, "<tr class='rule'><td>Total download packets</td><td>");
//                        Util_printRule(res.outputbuffer, bl.action, "If %s %lld packets in last %d %s(s)", operatornames[bl.operator], bl.limit, bl.rangecount, Util_timestr(bl.range));
//                }
//                StringBuffer_append(res.outputbuffer, "</td></tr>");
//        }
//}


        private void print_service_rules_uptime(StringBuilder res, Service_T s)
        {
            foreach (var ul in s.uptimelist)
            {
                res.Append("<tr class='rule'><td>Uptime</td><td>");
                Util.PrintRule(res, ul.action, "If {0} {1} second(s)", MonitWindowsAgent.operatornames[ul.compOperator],
                    ul.uptime);
                res.Append("</td></tr>");
            }
        }

//static void print_service_rules_match(HttpResponse res, Service_T s) {
//        if (s.type != Service_Process) {
//                for (Match_T ml = s.matchignorelist; ml; ml = ml.next) {
//                        StringBuffer_append(res.outputbuffer, "<tr class='rule'><td>Ignore pattern</td><td>");
//                        Util_printRule(res.outputbuffer, ml.action, "If %smatch \"%s\"", ml.not ? "not " : "", ml.match_string);
//                        StringBuffer_append(res.outputbuffer, "</td></tr>");
//                }
//                for (Match_T ml = s.matchlist; ml; ml = ml.next) {
//                        StringBuffer_append(res.outputbuffer, "<tr class='rule'><td>Pattern</td><td>");
//                        Util_printRule(res.outputbuffer, ml.action, "If %smatch \"%s\"", ml.not ? "not " : "", ml.match_string);
//                        StringBuffer_append(res.outputbuffer, "</td></tr>");
//                }
//        }
//}


//static void print_service_rules_checksum(HttpResponse res, Service_T s) {
//        if (s.checksum) {
//                StringBuffer_append(res.outputbuffer, "<tr class='rule'><td>Checksum</td><td>");
//                if (s.checksum.test_changes)
//                        Util_printRule(res.outputbuffer, s.checksum.action, "If changed %s", checksumnames[s.checksum.type]);
//                else
//                        Util_printRule(res.outputbuffer, s.checksum.action, "If failed %s(%s)", s.checksum.hash, checksumnames[s.checksum.type]);
//                StringBuffer_append(res.outputbuffer, "</td></tr>");
//        }
//}


//static void print_service_rules_pid(HttpResponse res, Service_T s) {
//        for (Pid_T l = s.pidlist; l; l = l.next) {
//                StringBuffer_append(res.outputbuffer, "<tr class='rule'><td>PID</td><td>");
//                Util_printRule(res.outputbuffer, l.action, "If changed");
//                StringBuffer_append(res.outputbuffer, "</td></tr>");
//        }
//}


//static void print_service_rules_ppid(HttpResponse res, Service_T s) {
//        for (Pid_T l = s.ppidlist; l; l = l.next) {
//                StringBuffer_append(res.outputbuffer, "<tr class='rule'><td>PPID</td><td>");
//                Util_printRule(res.outputbuffer, l.action, "If changed");
//                StringBuffer_append(res.outputbuffer, "</td></tr>");
//        }
//}


//static void print_service_rules_program(HttpResponse res, Service_T s) {
//        if (s.type == Service_Program) {
//                StringBuffer_append(res.outputbuffer, "<tr class='rule'><td>Program timeout</td><td>Terminate the program if not finished within %d seconds</td></tr>", s.program.timeout);
//                for (Status_T status = s.statuslist; status; status = status.next) {
//                        StringBuffer_append(res.outputbuffer, "<tr class='rule'><td>Test Exit value</td><td>");
//                        if (status.operator == Operator_Changed)
//                                Util_printRule(res.outputbuffer, status.action, "If exit value changed");
//                        else
//                                Util_printRule(res.outputbuffer, status.action, "If exit value %s %d", operatorshortnames[status.operator], status.return_value);
//                        StringBuffer_append(res.outputbuffer, "</td></tr>");
//                }
//        }
//}


        private void print_service_rules_resource(StringBuilder res, Service_T s)
        {
            foreach (var q in s.resourcelist)
            {
                res.Append("<tr class='rule'><td>");
                switch (q.resource_id)
                {
                    case MonitResourceType.Resource_CpuPercent:
                        res.Append("CPU usage limit");
                        break;

                    case MonitResourceType.Resource_CpuPercentTotal:
                        res.Append("CPU usage limit (incl. children)");
                        break;

                    case MonitResourceType.Resource_CpuUser:
                        res.Append("CPU user limit");
                        break;

                    case MonitResourceType.Resource_CpuSystem:
                        res.Append("CPU system limit");
                        break;

                    case MonitResourceType.Resource_CpuWait:
                        res.Append("CPU wait limit");
                        break;

                    case MonitResourceType.Resource_MemoryPercent:
                        res.Append("Memory usage limit");
                        break;

                    case MonitResourceType.Resource_MemoryKbyte:
                        res.Append("Memory amount limit");
                        break;

                    case MonitResourceType.Resource_SwapPercent:
                        res.Append("Swap usage limit");
                        break;

                    case MonitResourceType.Resource_SwapKbyte:
                        res.Append("Swap amount limit");
                        break;

                    case MonitResourceType.Resource_LoadAverage1m:
                        res.Append("Load average (1min)");
                        break;

                    case MonitResourceType.Resource_LoadAverage5m:
                        res.Append("Load average (5min)");
                        break;

                    case MonitResourceType.Resource_LoadAverage15m:
                        res.Append("Load average (15min)");
                        break;

                    case MonitResourceType.Resource_Children:
                        res.Append("Children");
                        break;

                    case MonitResourceType.Resource_MemoryKbyteTotal:
                        res.Append("Memory amount limit (incl. children)");
                        break;

                    case MonitResourceType.Resource_MemoryPercentTotal:
                        res.Append("Memory usage limit (incl. children)");
                        break;
                    default:
                        break;
                }
                res.Append("</td><td>");
                switch (q.resource_id)
                {
                    case MonitResourceType.Resource_CpuPercent:
                    case MonitResourceType.Resource_CpuPercentTotal:
                    case MonitResourceType.Resource_MemoryPercentTotal:
                    case MonitResourceType.Resource_CpuUser:
                    case MonitResourceType.Resource_CpuSystem:
                    case MonitResourceType.Resource_CpuWait:
                    case MonitResourceType.Resource_MemoryPercent:
                    case MonitResourceType.Resource_SwapPercent:
                        Util.PrintRule(res, q.action, "If {0} {1}%", MonitWindowsAgent.operatornames[q.compOperator],
                            (q.limit/10f).ToString());
                        break;

                    case MonitResourceType.Resource_MemoryKbyte:
                    case MonitResourceType.Resource_SwapKbyte:
                    case MonitResourceType.Resource_MemoryKbyteTotal:
                        Util.PrintRule(res, q.action, "If {0} {1}", MonitWindowsAgent.operatornames[q.compOperator],
                            (q.limit*1024).ToString());
                        break;

                    case MonitResourceType.Resource_LoadAverage1m:
                    case MonitResourceType.Resource_LoadAverage5m:
                    case MonitResourceType.Resource_LoadAverage15m:
                        Util.PrintRule(res, q.action, "If {0} {1}", MonitWindowsAgent.operatornames[q.compOperator],
                            (q.limit/10f).ToString());
                        break;

                    case MonitResourceType.Resource_Children:
                        Util.PrintRule(res, q.action, "If {0} {1}", MonitWindowsAgent.operatornames[q.compOperator],
                            q.limit.ToString());
                        break;
                    default:
                        break;
                }
                res.Append("</td></tr>");
            }
        }


//static void print_service_status_port(HttpResponse res, Service_T s) {
//        int status = Util_hasServiceStatus(s);
//        for (Port_T p = s.portlist; p; p = p.next) {
//                StringBuffer_append(res.outputbuffer, "<tr><td>Port Response time</td>");
//                if (! status)
//                        StringBuffer_append(res.outputbuffer, "<td>-<td>");
//                else if (! p.is_available)
//                        StringBuffer_append(res.outputbuffer, "<td class='red-text'>failed to [%s]:%d%s type %s/%s protocol %s</td>", p.hostname, p.port, p.request ? p.request : "", Util_portTypeDescription(p), Util_portIpDescription(p), p.protocol.name);
//                else
//                        StringBuffer_append(res.outputbuffer, "<td>%.3fs to %s:%d%s type %s/%s protocol %s</td>", p.response, p.hostname, p.port, p.request ? p.request : "", Util_portTypeDescription(p), Util_portIpDescription(p), p.protocol.name);
//                StringBuffer_append(res.outputbuffer, "</tr>");
//        }
//}


//static void print_service_status_socket(HttpResponse res, Service_T s) {
//        int status = Util_hasServiceStatus(s);
//        for (Port_T p = s.socketlist; p; p = p.next) {
//                StringBuffer_append(res.outputbuffer, "<tr><td>Unix Socket Response time</td>");
//                if (! status)
//                        StringBuffer_append(res.outputbuffer, "<td>-<td>");
//                else if (! p.is_available)
//                        StringBuffer_append(res.outputbuffer, "<td class='red-text'>failed to %s type %s protocol %s</td>", p.pathname, Util_portTypeDescription(p), p.protocol.name);
//                else
//                        StringBuffer_append(res.outputbuffer, "<td>%.3fs to %s type %s protocol %s</td>", p.response, p.pathname, Util_portTypeDescription(p), p.protocol.name);
//                StringBuffer_append(res.outputbuffer, "</tr>");
//        }
//}


//static void print_service_status_icmp(HttpResponse res, Service_T s) {
//        int status = Util_hasServiceStatus(s);
//        for (Icmp_T i = s.icmplist; i; i = i.next) {
//                StringBuffer_append(res.outputbuffer, "<tr><td>Ping Response time</td>");
//                if (! status)
//                        StringBuffer_append(res.outputbuffer, "<td>-</td>");
//                else if (! i.is_available)
//                        StringBuffer_append(res.outputbuffer, "<td class='red-text'>connection failed</td>");
//                else if (i.response < 0)
//                        StringBuffer_append(res.outputbuffer, "<td class='gray-text'>N/A</td>");
//                else
//                        StringBuffer_append(res.outputbuffer, "<td>%.3fs</td>", i.response);
//                StringBuffer_append(res.outputbuffer, "</tr>");
//        }
//}


        private void print_service_status_perm(StringBuilder res, Service_T s, ushort mode)
        {
            res.Append("<tr><td>Permission</td>");
            if (!Util.HasServiceStatus(s))
                res.Append("<td>-</td>");
            else
                res.AppendFormat("<td class='{0}'>{1}</td>",
                    (s.error & (int) MonitEventType.Event_Permission) != 0 ? "red-text" : "", mode);
            res.Append("</tr>");
        }


//static void print_service_status_uid(HttpResponse res, Service_T s, uid_t uid) {
//        StringBuffer_append(res.outputbuffer, "<tr><td>UID</td>");
//        if (! Util_hasServiceStatus(s))
//                StringBuffer_append(res.outputbuffer, "<td>-</td>");
//        else
//                StringBuffer_append(res.outputbuffer, "<td class='%s'>%d</td>", (s.error & Event_Uid) ? "red-text" : "", (int)uid);
//        StringBuffer_append(res.outputbuffer, "</tr>");
//}


//static void print_service_status_gid(HttpResponse res, Service_T s, gid_t gid) {
//        StringBuffer_append(res.outputbuffer, "<tr><td>GID</td>");
//        if (! Util_hasServiceStatus(s))
//                StringBuffer_append(res.outputbuffer, "<td>-</td>");
//        else
//                StringBuffer_append(res.outputbuffer, "<td class='%s'>%d</td>", (s.error & Event_Gid) ? "red-text" : "", (int)gid);
//        StringBuffer_append(res.outputbuffer, "</tr>");
//}


//static void print_service_status_link(HttpResponse res, Service_T s) {
//        StringBuffer_append(res.outputbuffer, "<tr><td>Link capacity</td>");
//        if (! Util_hasServiceStatus(s) || Link_getState(s.inf.priv.net.stats) != 1) {
//                StringBuffer_append(res.outputbuffer, "<td>-</td>");
//        } else {
//                long long speed = Link_getSpeed(s.inf.priv.net.stats);
//                if (speed > 0)
//                        StringBuffer_append(res.outputbuffer, "<td class='%s'>%.0lf Mb&#47;s %s-duplex</td>", s.error & Event_Speed ? "red-text" : "", (double)speed / 1000000., Link_getDuplex(s.inf.priv.net.stats) == 1 ? "full" : "half");
//                else
//                        StringBuffer_append(res.outputbuffer, "<td class='gray-text'>N/A for this link type</td>");
//        }
//        StringBuffer_append(res.outputbuffer, "</tr>");
//}


//static void print_service_status_download(HttpResponse res, Service_T s) {
//        StringBuffer_append(res.outputbuffer, "<tr><td>Download</td>");
//        if (! Util_hasServiceStatus(s) || Link_getState(s.inf.priv.net.stats) != 1) {
//                StringBuffer_append(res.outputbuffer, "<td>-</td>");
//        } else {
//                char buf[STRLEN];
//                long long speed = Link_getSpeed(s.inf.priv.net.stats);
//                long long ibytes = Link_getBytesInPerSecond(s.inf.priv.net.stats);
//                StringBuffer_append(res.outputbuffer, "<td class='%s'>%s/s [%lld packets/s] [%lld errors]",
//                                    (s.error & Event_ByteIn || s.error & Event_PacketIn) ? "red-text" : "",
//                                    Str_bytesToSize(ibytes, buf),
//                                    Link_getPacketsInPerSecond(s.inf.priv.net.stats),
//                                    Link_getErrorsInPerSecond(s.inf.priv.net.stats));
//                if (speed > 0 && ibytes > 0)
//                        StringBuffer_append(res.outputbuffer, " (%.1f%% link saturation)", 100. * ibytes * 8 / (double)speed);
//                StringBuffer_append(res.outputbuffer, "</td>");
//        }
//        StringBuffer_append(res.outputbuffer, "</tr>");
//}


//static void print_service_status_upload(HttpResponse res, Service_T s) {
//        StringBuffer_append(res.outputbuffer, "<tr><td>Upload</td>");
//        if (! Util_hasServiceStatus(s) || Link_getState(s.inf.priv.net.stats) != 1) {
//                StringBuffer_append(res.outputbuffer, "<td>-</td>");
//        } else {
//                char buf[STRLEN];
//                long long speed = Link_getSpeed(s.inf.priv.net.stats);
//                long long obytes = Link_getBytesOutPerSecond(s.inf.priv.net.stats);
//                StringBuffer_append(res.outputbuffer, "<td class='%s'>%s/s [%lld packets/s] [%lld errors]",
//                                    (s.error & Event_ByteOut || s.error & Event_PacketOut) ? "red-text" : "",
//                                    Str_bytesToSize(obytes, buf),
//                                    Link_getPacketsOutPerSecond(s.inf.priv.net.stats),
//                                    Link_getErrorsOutPerSecond(s.inf.priv.net.stats));
//                if (speed > 0 && obytes > 0)
//                        StringBuffer_append(res.outputbuffer, " (%.1f%% link saturation)", 100. * obytes * 8 / (double)speed);
//                StringBuffer_append(res.outputbuffer, "</td>");
//        }
//        StringBuffer_append(res.outputbuffer, "</tr>");
//}


//static void print_service_status_timestamp(HttpResponse res, Service_T s, time_t timestamp) {
//        StringBuffer_append(res.outputbuffer, "<tr><td>Timestamp</td>");
//        if (! Util_hasServiceStatus(s)) {
//                StringBuffer_append(res.outputbuffer, "<td>-</td>");
//        } else {
//                char t[32];
//                StringBuffer_append(res.outputbuffer, "<td class='%s'>%s</td>", (s.error & Event_Timestamp) ? "red-text" : "", Time_string(timestamp, t));
//        }
//        StringBuffer_append(res.outputbuffer, "</tr>");
//}


//static void print_service_status_filesystem_flags(HttpResponse res, Service_T s) {
//        StringBuffer_append(res.outputbuffer, "<tr><td>Filesystem flags</td>");
//        if (! Util_hasServiceStatus(s))
//                StringBuffer_append(res.outputbuffer, "<td>-</td>");
//        else
//                StringBuffer_append(res.outputbuffer, "<td>0x%x</td>", s.inf.priv.filesystem.flags);
//        StringBuffer_append(res.outputbuffer, "</tr>");
//}


        private void print_service_status_filesystem_blockstotal(StringBuilder res, Service_T s)
        {
            res.Append("<tr><td>Space total</td>");
            if (!Util.HasServiceStatus(s))
            {
                res.Append("<td>-</td>");
            }
            else
            {
                var filesystem = s.inf as FileSystemInfo_T;
                res.AppendFormat("<td>{0} (of which {1}% is reserved for root user)</td>",
                    filesystem.f_bsize > 0 ? (filesystem.f_blocks*filesystem.f_bsize).ToString() : "0 MB",
                    filesystem.f_blocks > 0
                        ? (100*(float) (filesystem.f_blocksfreetotal - filesystem.f_blocksfree)/
                           filesystem.f_blocks)
                        : 0);
            }
            res.Append("</tr>");
        }


        private void print_service_status_filesystem_blocksfree(StringBuilder res, Service_T s)
        {
            res.Append("<tr><td>Space free for non superuser</td>");
            if (!Util.HasServiceStatus(s))
            {
                res.Append("<td>-</td>");
            }
            else
            {
                var filesystem = s.inf as FileSystemInfo_T;
                res.AppendFormat("<td>{0} [{1}%]</td>",
                    filesystem.f_bsize > 0 ? (filesystem.f_blocksfree*filesystem.f_bsize).ToString() : "0 MB",
                    filesystem.f_blocks > 0
                        ? (100*(float) filesystem.f_blocksfree/filesystem.f_blocks)
                        : 0);
            }
            res.Append("</tr>");
        }


        private void print_service_status_filesystem_blocksfreetotal(StringBuilder res, Service_T s)
        {
            res.Append("<tr><td>Space free total</td>");
            if (!Util.HasServiceStatus(s))
            {
                res.Append("<td>-</td>");
            }
            else
            {
                var filesystem = s.inf as FileSystemInfo_T;
                res.AppendFormat(
                    "<td class='{0}'>{1} [{2}%]</td>",
                    (s.error & (int) MonitEventType.Event_Resource) != 0 ? "red-text" : "",
                    filesystem.f_bsize > 0 ? (filesystem.f_blocksfreetotal*filesystem.f_bsize).ToString() : "0 MB",
                    filesystem.f_blocks > 0
                        ? (100*(float) filesystem.f_blocksfreetotal/filesystem.f_blocks)
                        : 0);
            }
            res.Append("</tr>");
        }


        private void print_service_status_filesystem_blocksize(StringBuilder res, Service_T s)
        {
            res.Append("<tr><td>Block size</td>");
            if (!Util.HasServiceStatus(s))
            {
                res.Append("<td>-</td>");
            }
            else
            {
                var filesystem = s.inf as FileSystemInfo_T;
                res.AppendFormat("<td>{0}</td>", filesystem.f_bsize);
            }
            res.Append("</tr>");
        }


        private void print_service_status_filesystem_inodestotal(StringBuilder res, Service_T s)
        {
            res.Append("<tr><td>Inodes total</td>");
            if (!Util.HasServiceStatus(s))
            {
                res.Append("<td>-</td>");
            }
            else
            {
                var filesystem = s.inf as FileSystemInfo_T;
                if (filesystem.f_files > 0)
                    res.AppendFormat("<td>{0}</td>", filesystem.f_files);
                else
                    res.AppendFormat("<td class='gray-text'>N/A</td>");
            }
            res.Append("</tr>");
        }


        private void print_service_status_filesystem_inodesfree(StringBuilder res, Service_T s)
        {
            res.Append("<tr><td>Inodes free</td>");
            if (!Util.HasServiceStatus(s))
            {
                res.Append("<td>-</td>");
            }
            else
            {
                var filesystem = s.inf as FileSystemInfo_T;
                if (filesystem.f_files > 0)
                    res.AppendFormat("<td class='{0}'>{1} [{2}%]</td>",
                        (s.error & (int) MonitEventType.Event_Resource) != 0 ? "red-text" : "", filesystem.f_filesfree,
                        100*(float) filesystem.f_filesfree/filesystem.f_files);
                else
                    res.AppendFormat("<td class='gray-text'>N/A</td>");
            }
            res.Append("</tr>");
        }


//static void print_service_status_file_size(HttpResponse res, Service_T s) {
//        StringBuffer_append(res.outputbuffer, "<tr><td>Size</td>");
//        if (! Util_hasServiceStatus(s)) {
//                StringBuffer_append(res.outputbuffer, "<td>-</td>");
//        } else {
//                char buf[STRLEN];
//                StringBuffer_append(res.outputbuffer, "<td class='%s'>%s</td>", (s.error & Event_Size) ? "red-text" : "", Str_bytesToSize(s.inf.priv.file.size, buf));
//        }
//        StringBuffer_append(res.outputbuffer, "</tr>");
//}

//static void print_service_status_file_match(HttpResponse res, Service_T s) {
//        StringBuffer_append(res.outputbuffer, "<tr><td>Match regex</td>");
//        if (! Util_hasServiceStatus(s))
//                StringBuffer_append(res.outputbuffer, "<td>-</td>");
//        else
//                StringBuffer_append(res.outputbuffer, "<td class='%s'>%s</td>", (s.error & Event_Content) ? "red-text" : "", (s.error & Event_Content) ? "yes" : "no");
//        StringBuffer_append(res.outputbuffer, "</tr>");
//}


//static void print_service_status_file_checksum(HttpResponse res, Service_T s) {
//        if (s.checksum) {
//                StringBuffer_append(res.outputbuffer, "<tr><td>Checksum</td>");
//                if (! Util_hasServiceStatus(s))
//                        StringBuffer_append(res.outputbuffer, "<td>-</td>");
//                else
//                        StringBuffer_append(res.outputbuffer, "<td class='%s'>%s(%s)</td>", (s.error & Event_Checksum) ? "red-text" : "", s.inf.priv.file.cs_sum, checksumnames[s.checksum.type]);
//                StringBuffer_append(res.outputbuffer, "</tr>");
//        }
//}


//static void print_service_status_process_pid(HttpResponse res, Service_T s) {
//        StringBuffer_append(res.outputbuffer, "<tr><td>Process id</td>");
//        if (! Util_hasServiceStatus(s))
//                StringBuffer_append(res.outputbuffer, "<td>-</td>");
//        else
//                StringBuffer_append(res.outputbuffer, "<td>%d</td>", s.inf.priv.process.pid > 0 ? s.inf.priv.process.pid : 0);
//        StringBuffer_append(res.outputbuffer, "</tr>");
//}


//static void print_service_status_process_ppid(HttpResponse res, Service_T s) {
//        StringBuffer_append(res.outputbuffer, "<tr><td>Parent process id</td>");
//        if (! Util_hasServiceStatus(s))
//                StringBuffer_append(res.outputbuffer, "<td>-</td>");
//        else
//                StringBuffer_append(res.outputbuffer, "<td>%d</td>", s.inf.priv.process.ppid > 0 ? s.inf.priv.process.ppid : 0);
//        StringBuffer_append(res.outputbuffer, "</tr>");
//}


//static void print_service_status_process_euid(HttpResponse res, Service_T s) {
//        StringBuffer_append(res.outputbuffer, "<tr><td>Effective UID</td>");
//        if (! Util_hasServiceStatus(s))
//                StringBuffer_append(res.outputbuffer, "<td>-</td>");
//        else
//                StringBuffer_append(res.outputbuffer, "<td>%d</td>", s.inf.priv.process.euid);
//        StringBuffer_append(res.outputbuffer, "</tr>");
//}


        private void print_service_status_process_uptime(StringBuilder res, Service_T s)
        {
            res.Append("<tr><td>Process uptime</td>");
            if (!Util.HasServiceStatus(s))
            {
                res.Append("<td>-</td>");
            }
            else
            {
                var process = s.inf as ProcessInfo_T;
                res.AppendFormat("<td>{0}</td>", process.uptime);
            }
            res.Append("</tr>");
        }


        private void print_service_status_process_children(StringBuilder res, Service_T s)
        {
            if (MonitWindowsAgent.Run.doprocess)
            {
                res.Append("<tr><td>Children</td>");
                if (!Util.HasServiceStatus(s))
                    res.Append("<td>-</td>");
                else
                {
                    var process = s.inf as ProcessInfo_T;
                    res.AppendFormat("<td class='{0}'>{1}</td>",
                        (s.error & (int) MonitEventType.Event_Resource) != 0 ? "red-text" : "", process.children);
                }
                res.Append("</tr>");
            }
        }


        private void print_service_status_process_cpu(StringBuilder res, Service_T s)
        {
            if (MonitWindowsAgent.Run.doprocess)
            {
                res.Append("<tr><td>CPU usage</td>");
                if (!Util.HasServiceStatus(s))
                    res.Append("<td>-</td>");
                else
                {
                    var process = s.inf as ProcessInfo_T;
                    res.AppendFormat("<td class='{0}'>{1}% (Usage / Number of CPUs)</td>",
                        (s.error & (int) MonitEventType.Event_Resource) != 0 ? "red-text" : "", process.cpu_percent);
                }
                res.Append("</tr>");
            }
        }


        private void print_service_status_process_cputotal(StringBuilder res, Service_T s)
        {
            if (MonitWindowsAgent.Run.doprocess)
            {
                res.Append("<tr><td>Total CPU usage (incl. children)</td>");
                if (!Util.HasServiceStatus(s))
                    res.Append("<td>-</td>");
                else
                {
                    var process = s.inf as ProcessInfo_T;
                    res.AppendFormat("<td class='{0}'>{1}%</td>",
                        (s.error & (int) MonitEventType.Event_Resource) != 0 ? "red-text" : "",
                        process.total_cpu_percent);
                }
                res.Append("</tr>");
            }
        }


        private void print_service_status_process_memory(StringBuilder res, Service_T s)
        {
            if (MonitWindowsAgent.Run.doprocess)
            {
                res.Append("<tr><td>Memory usage</td>");
                if (!Util.HasServiceStatus(s))
                {
                    res.Append("<td>-</td>");
                }
                else
                {
                    var process = s.inf as ProcessInfo_T;
                    res.AppendFormat("<td class='{0}'>{1}% [{2}]</td>",
                        (s.error & (int) MonitEventType.Event_Resource) != 0 ? "red-text" : "", process.mem_percent,
                        (process.mem_kbyte*1024));
                }
                res.Append("</tr>");
            }
        }


        private void print_service_status_process_memorytotal(StringBuilder res, Service_T s)
        {
            if (MonitWindowsAgent.Run.doprocess)
            {
                res.Append("<tr><td>Total memory usage (incl. children)</td>");
                if (!Util.HasServiceStatus(s))
                {
                    res.Append("<td>-</td>");
                }
                else
                {
                    var process = s.inf as ProcessInfo_T;
                    res.AppendFormat("<td class='{0}'>{1}% [{2}]</td>",
                        (s.error & (int) MonitEventType.Event_Resource) != 0 ? "red-text" : "",
                        process.total_mem_percent, (process.total_mem_kbyte*1024));
                }
                res.Append("</tr>");
            }
        }


        private void print_service_status_system_loadavg(StringBuilder res, Service_T s)
        {
            res.Append("<tr><td>Load average</td>");
            if (!Util.HasServiceStatus(s))
                res.Append("<td>-</td>");
            else
                res.AppendFormat("<td class='{0}'>[{1}] [{2}] [{3}]</td>",
                    (s.error & (int) MonitEventType.Event_Resource) != 0 ? "red-text" : "",
                    MonitWindowsAgent.systeminfo.loadavg[0], MonitWindowsAgent.systeminfo.loadavg[1],
                    MonitWindowsAgent.systeminfo.loadavg[2]);
            res.Append("</tr>");
        }


        private void print_service_status_system_cpu(StringBuilder res, Service_T s)
        {
            res.Append("<tr><td>CPU usage</td>");
            if (!Util.HasServiceStatus(s))
            {
                res.Append("<td>-</td>");
            }
            else
            {
                res.AppendFormat("<td class='{0}'>{1}%us {2}%sy {3}%wa{4}",
                    (s.error & (int) MonitEventType.Event_Resource) != 0 ? "red-text" : "",
                    MonitWindowsAgent.systeminfo.total_cpu_user_percent > 0
                        ? MonitWindowsAgent.systeminfo.total_cpu_user_percent
                        : 0,
                    MonitWindowsAgent.systeminfo.total_cpu_syst_percent > 0
                        ? MonitWindowsAgent.systeminfo.total_cpu_syst_percent
                        : 0,
                    MonitWindowsAgent.systeminfo.total_cpu_wait_percent > 0
                        ? MonitWindowsAgent.systeminfo.total_cpu_wait_percent
                        : 0, "</td>");
            }
            res.Append("</tr>");
        }


        private void print_service_status_system_memory(StringBuilder res, Service_T s)
        {
            res.Append("<tr><td>Memory usage</td>");
            if (!Util.HasServiceStatus(s))
            {
                res.Append("<td>-</td>");
            }
            else
            {
                res.AppendFormat("<td class='{0}'>{1} [{2}%]</td>",
                    (s.error & (int) MonitEventType.Event_Resource) != 0 ? "red-text" : "",
                    (MonitWindowsAgent.systeminfo.total_mem_kbyte*1024),
                    (MonitWindowsAgent.systeminfo.total_mem_percent));
            }
            res.Append("</tr>");
        }


        private void print_service_status_system_swap(StringBuilder res, Service_T s)
        {
            res.Append("<tr><td>Swap usage</td>");
            if (!Util.HasServiceStatus(s))
            {
                res.Append("<td>-</td>");
            }
            else
            {
                res.AppendFormat("<td class='{0}'>{1} [{2}%]</td>",
                    (s.error & (int) MonitEventType.Event_Resource) != 0 ? "red-text" : "",
                    (MonitWindowsAgent.systeminfo.total_swap_kbyte*1024),
                    (MonitWindowsAgent.systeminfo.total_swap_percent));
            }
            res.Append("</tr>");
        }


        private static void print_service_status_program_started(StringBuilder res, Service_T s)
        {
            res.Append("<tr><td>Last started</td>");
            if (!Util.HasServiceStatus(s))
            {
                res.Append("<td>-</td>");
            }
            else
            {
                if (s.program.started != null)
                {
                    res.AppendFormat("<td>{0}</td>", Timing.GetTimestamp(s.program.started));
                }
                else
                {
                    res.Append("<td>Not yet started</td>");
                }
            }
            res.Append("</tr>");
        }


        private static void print_service_status_program_status(StringBuilder res, Service_T s)
        {
            res.Append("<tr><td>Last exit value</td>");
            if (!Util.HasServiceStatus(s))
            {
                res.Append("<td>-</td>");
            }
            else
            {
                if (s.program.started != null)
                    res.AppendFormat("<td>{0}</td>", s.program.exitStatus);
                else
                    res.Append("<td class='gray-text'>N/A</td>");
            }
            res.Append("</tr>");
        }


        private static void print_service_status_program_output(StringBuilder res, Service_T s)
        {
            res.Append("<tr><td>Last output</td>");
            if (!Util.HasServiceStatus(s))
            {
                res.Append("<td>-</td>");
            }
            else
            {
                if (s.program.started != null)
                {
                    res.Append("<td>");
                    if (s.program.output.Length > 0)
                    {
                        // If the output contains multiple line, wrap use <pre>, otherwise keep as is
                        var multiline = s.program.output.ToString().LastIndexOf("\n") > 0;
                        if (multiline)
                            res.Append("<pre>");
                        res.Append(Processor.EscapeHTML(s.program.output.ToString()));
                        if (multiline)
                            res.Append("</pre>");
                    }
                    else
                    {
                        res.Append("no output");
                    }
                    res.Append("</td>");
                }
                else
                {
                    res.Append("<td class='gray-text'>N/A</td>");
                }
            }
            res.Append("</tr>");
        }

        #endregion

        #region Status Output

        /* Print status in the given format. Text status is default. */

        private void printStatus(HttpRequest req, HttpResponse res, int version)
        {
            var responseString = new StringBuilder();
            var level = MonitLevelType.Level_Full;
            var stringFormat = Processor.GetParameter(req, "format");
            var stringLevel = Processor.GetParameter(req, "level");

            if (!string.IsNullOrEmpty(stringLevel) && stringLevel.StartsWith("summary"))
                level = MonitLevelType.Level_Summary;

            if (!string.IsNullOrEmpty(stringFormat) && stringFormat.StartsWith("xml"))
            {
                responseString.Append(Xml.StatusXml(null, level, version, "TODO: LocalHostName"));
                res.ContentType = "text/xml";
            }
            else
            {
                //string uptime = Util_getUptime(getProcessUptime(getpid(), ptree, ptreesize), " ");
                responseString.AppendFormat("The Monit daemon {0} uptime: {1}\n\n", Processor.SERVER_VERSION, ""
                    /*uptime*/);

                foreach (var s in MonitWindowsAgent.servicelist_conf)
                    status_service_txt(s, responseString, level);
                res.ContentType = "text/plain";
            }

            using (var writer = new StreamWriter(res.OutputStream))
            {
                writer.Write(responseString.ToString());
            }
        }


        private void status_service_txt(Service_T s, StringBuilder res, MonitLevelType level)
        {
            if (level == MonitLevelType.Level_Summary)
            {
                res.AppendFormat("{0} {1}\n",
                    txtFormatting(string.Format("{0} '{1}'", MonitWindowsAgent.servicetypes[(int) s.type], s.name), 35),
                    getServiceStatus(s));
            }
            else
            {
                res.AppendFormat("{0} '{1}'\n  {2} {3}\n", MonitWindowsAgent.servicetypes[(int) s.type], s.name,
                    txtFormatting("status", 33), getServiceStatus(s));
                res.AppendFormat("  {0} {1}\n", txtFormatting("monitoring status", 33), getMonitoringStatus(s));

                if (Util.HasServiceStatus(s))
                {
                    switch (s.type)
                    {
                        case MonitServiceType.Service_File:
                            //StringBuffer_append(res.outputbuffer,
                            //            "  %-33s %o\n"
                            //            "  %-33s %d\n"
                            //            "  %-33s %d\n"
                            //            "  %-33s %s\n"
                            //            "  %-33s %s\n",
                            //            "permission", s.inf.priv.file.mode & 07777,
                            //            "uid", (int)s.inf.priv.file.uid,
                            //            "gid", (int)s.inf.priv.file.gid,
                            //            "size", Str_bytesToSize(s.inf.priv.file.size, buf),
                            //            "timestamp", Time_string(s.inf.priv.file.timestamp, buf));
                            //if (s.checksum) {
                            //        StringBuffer_append(res.outputbuffer,
                            //                            "  %-33s %s (%s)\n",
                            //                            "checksum", s.inf.priv.file.cs_sum,
                            //                            checksumnames[s.checksum.type]);
                            //}
                            break;

                        case MonitServiceType.Service_Directory:
                            //StringBuffer_append(res.outputbuffer,
                            //            "  %-33s %o\n"
                            //            "  %-33s %d\n"
                            //            "  %-33s %d\n"
                            //            "  %-33s %s\n",
                            //            "permission", s.inf.priv.directory.mode & 07777,
                            //            "uid", (int)s.inf.priv.directory.uid,
                            //            "gid", (int)s.inf.priv.directory.gid,
                            //            "timestamp", Time_string(s.inf.priv.directory.timestamp, buf));
                            break;

                        case MonitServiceType.Service_Fifo:
                            //StringBuffer_append(res.outputbuffer,
                            //            "  %-33s %o\n"
                            //            "  %-33s %d\n"
                            //            "  %-33s %d\n"
                            //            "  %-33s %s\n",
                            //            "permission", s.inf.priv.fifo.mode & 07777,
                            //            "uid", (int)s.inf.priv.fifo.uid,
                            //            "gid", (int)s.inf.priv.fifo.gid,
                            //            "timestamp", Time_string(s.inf.priv.fifo.timestamp, buf));
                            break;

                        case MonitServiceType.Service_Net:
                            //if (Link_getState(s.inf.priv.net.stats) == 1) {
                            //        long long speed = Link_getSpeed(s.inf.priv.net.stats);
                            //        if (speed > 0)
                            //                StringBuffer_append(res.outputbuffer,
                            //                                    "  %-33s %.0lf Mb/s %s-duplex\n",
                            //                                    "link capacity", (double)speed / 1000000., Link_getDuplex(s.inf.priv.net.stats) == 1 ? "full" : "half");
                            //        else
                            //                StringBuffer_append(res.outputbuffer,
                            //                                    "  %-33s N/A for this link type\n",
                            //                                    "link capacity");

                            //        long long ibytes = Link_getBytesInPerSecond(s.inf.priv.net.stats);
                            //        StringBuffer_append(res.outputbuffer, "  %-33s %s/s [%lld packets/s] [%lld errors]",
                            //                            "download",
                            //                            Str_bytesToSize(ibytes, buf),
                            //                            Link_getPacketsInPerSecond(s.inf.priv.net.stats),
                            //                            Link_getErrorsInPerSecond(s.inf.priv.net.stats));
                            //        if (speed > 0 && ibytes > 0)
                            //                StringBuffer_append(res.outputbuffer, " (%.1f%% link saturation)", 100. * ibytes * 8 / (double)speed);
                            //        StringBuffer_append(res.outputbuffer, "\n");

                            //        long long obytes = Link_getBytesOutPerSecond(s.inf.priv.net.stats);
                            //        StringBuffer_append(res.outputbuffer, "  %-33s %s/s [%lld packets/s] [%lld errors]",
                            //                            "upload",
                            //                            Str_bytesToSize(obytes, buf),
                            //                            Link_getPacketsOutPerSecond(s.inf.priv.net.stats),
                            //                            Link_getErrorsOutPerSecond(s.inf.priv.net.stats));
                            //        if (speed > 0 && obytes > 0)
                            //                StringBuffer_append(res.outputbuffer, " (%.1f%% link saturation)", 100. * obytes * 8 / (double)speed);
                            //        StringBuffer_append(res.outputbuffer, "\n");
                            //}
                            break;

                        case MonitServiceType.Service_Filesystem:
                            var filesystem = s.inf as FileSystemInfo_T;
                            res.AppendFormat("  {0} {1}\n  {2} {3}\n  {4} {5}\n", txtFormatting("permission", 33),
                                filesystem.mode, txtFormatting("uid", 33), filesystem.uid,
                                txtFormatting("gid", 33), filesystem.gid);
                            res.AppendFormat("  {0} 0x{1}\n  {2} {3}\n", txtFormatting("filesystem flags", 33),
                                filesystem.flags, txtFormatting("block size", 33), (filesystem.f_bsize));
                            res.AppendFormat("  {0} {1} (of which {2}% is reserved for root user)\n",
                                txtFormatting("space total", 33),
                                filesystem.f_bsize > 0
                                    ? (filesystem.f_blocks*filesystem.f_bsize).ToString()
                                    : "0 MB",
                                filesystem.f_blocks > 0
                                    ? (100*(float) (filesystem.f_blocksfreetotal - filesystem.f_blocksfree)/
                                       filesystem.f_blocks)
                                    : 0);
                            res.AppendFormat("  {0} {1} [{2}%]\n", txtFormatting("space free for non superuser", 33),
                                filesystem.f_bsize > 0
                                    ? (filesystem.f_blocksfree*filesystem.f_bsize).ToString()
                                    : "0 MB",
                                filesystem.f_blocks > 0
                                    ? (100*(float) filesystem.f_blocksfree/filesystem.f_blocks)
                                    : 0);
                            res.AppendFormat("  {0} {1} [{2}%]\n", txtFormatting("space free total", 33),
                                filesystem.f_bsize > 0
                                    ? (filesystem.f_blocksfreetotal*filesystem.f_bsize).ToString()
                                    : "0 MB",
                                filesystem.f_blocks > 0
                                    ? (100*(float) filesystem.f_blocksfreetotal/filesystem.f_blocks)
                                    : 0);
                            if (filesystem.f_files > 0)
                            {
                                res.AppendFormat("  {0} {1}\n  {2} {3} [{4}%]\n", txtFormatting("inodes total", 33),
                                    filesystem.f_files, txtFormatting("inodes free", 33), filesystem.f_filesfree,
                                    ((float) 100*(float) filesystem.f_filesfree/(float) filesystem.f_files));
                            }
                            break;

                        case MonitServiceType.Service_Process:
                            var process = s.inf as ProcessInfo_T;
                            //uptime = Util_getUptime(process.uptime, " ");
                            res.AppendFormat("  {0} {1}\n  {2} {3}\n  {4} {5}\n  {6} {7}\n  {8} {9}\n  {10} {11}\n",
                                txtFormatting("pid", 33), process.pid > 0 ? process.pid : 0,
                                txtFormatting("parent pid", 33), process.ppid > 0 ? process.ppid : 0,
                                txtFormatting("uid", 33), process.uid,
                                txtFormatting("effective uid", 33), process.euid,
                                txtFormatting("gid", 33), process.gid,
                                txtFormatting("uptime", 33), process.uptime);
                            if (MonitWindowsAgent.Run.doprocess)
                            {
                                res.AppendFormat("  {0} {1}\n", txtFormatting("children", 33), process.children);
                                res.AppendFormat("  {0} {1}\n", txtFormatting("memory", 33),
                                    (process.mem_kbyte*1024));
                                res.AppendFormat("  {0} {1}\n", txtFormatting("memory total", 33),
                                    (process.total_mem_kbyte*1024));
                                res.AppendFormat("  {0} {1}%\n  {2} {3}%\n  {4} {5}%\n  {6} {7}%\n",
                                    txtFormatting("memory percent", 33), process.mem_percent,
                                    txtFormatting("memory percent total", 33), process.total_mem_percent,
                                    txtFormatting("cpu percent", 33), process.cpu_percent,
                                    txtFormatting("cpu percent total", 33), process.total_cpu_percent);
                            }
                            break;
                        default:
                            break;
                    }
                    //                        for (Icmp_T i = s.icmplist; i; i = i.next) {
                    //                                if (! i.is_available)
                    //                                        StringBuffer_append(res.outputbuffer,
                    //                                                            "  %-33s connection failed\n",
                    //                                                            "ping response time");
                    //                                else if (i.response < 0)
                    //                                        StringBuffer_append(res.outputbuffer,
                    //                                                            "  %-33s N/A\n",
                    //                                                            "ping response time");
                    //                                else
                    //                                        StringBuffer_append(res.outputbuffer,
                    //                                                            "  %-33s %.3fs\n",
                    //                                                            "ping response time", i.response);
                    //                        }
                    //                        for (Port_T p = s.portlist; p; p = p.next) {
                    //                                if (p.is_available)
                    //                                        StringBuffer_append(res.outputbuffer,
                    //                                                    "  %-33s %.3fs to [%s]:%d%s type %s/%s protocol %s\n",
                    //                                                    "port response time", p.response, p.hostname, p.port, p.request ? p.request : "", Util_portTypeDescription(p), Util_portIpDescription(p), p.protocol.name);
                    //                                else
                    //                                        StringBuffer_append(res.outputbuffer,
                    //                                                    "  %-33s FAILED to [%s]:%d%s type %s/%s protocol %s\n",
                    //                                                    "port response time", p.hostname, p.port, p.request ? p.request : "", Util_portTypeDescription(p), Util_portIpDescription(p), p.protocol.name);
                    //                        }
                    //                        for (Port_T p = s.socketlist; p; p = p.next) {
                    //                                if (p.is_available)
                    //                                        StringBuffer_append(res.outputbuffer,
                    //                                                    "  %-33s %.3fs to %s type %s protocol %s\n",
                    //                                                    "unix socket response time", p.response, p.pathname, Util_portTypeDescription(p), p.protocol.name);
                    //                                else
                    //                                        StringBuffer_append(res.outputbuffer,
                    //                                                    "  %-33s FAILED to %s type %s protocol %s\n",
                    //                                                    "unix socket response time", p.pathname, Util_portTypeDescription(p), p.protocol.name);
                    //                        }
                    if (s.type == MonitServiceType.Service_System && MonitWindowsAgent.Run.doprocess)
                    {
                        res.AppendFormat("  {0} [{1}] [{2}] [{3}]\n  {4}\t {5}%us {6}%sy {7}%wa\n",
                            txtFormatting("load average", 33), MonitWindowsAgent.systeminfo.loadavg[0],
                            MonitWindowsAgent.systeminfo.loadavg[1], MonitWindowsAgent.systeminfo.loadavg[2], "cpu",
                            MonitWindowsAgent.systeminfo.total_cpu_user_percent > 0
                                ? MonitWindowsAgent.systeminfo.total_cpu_user_percent
                                : 0,
                            MonitWindowsAgent.systeminfo.total_cpu_syst_percent > 0
                                ? MonitWindowsAgent.systeminfo.total_cpu_syst_percent
                                : 0,
                            MonitWindowsAgent.systeminfo.total_cpu_wait_percent > 0
                                ? MonitWindowsAgent.systeminfo.total_cpu_wait_percent
                                : 0);
                        res.AppendFormat("  {0} {1} [{2}%]\n", txtFormatting("memory usage", 33),
                            (MonitWindowsAgent.systeminfo.total_mem_kbyte*1024),
                            MonitWindowsAgent.systeminfo.total_mem_percent);
                        res.AppendFormat("  {0} {1} [{2}%]\n", txtFormatting("swap usage", 33),
                            (MonitWindowsAgent.systeminfo.total_swap_kbyte*1024f),
                            MonitWindowsAgent.systeminfo.total_swap_percent);
                    }
                    //                        if (s.type == Service_Program) {
                    //                                if (s.program.started) {
                    //                                        char t[32];
                    //                                        StringBuffer_append(res.outputbuffer,
                    //                                                            "  %-33s %s\n"
                    //                                                            "  %-33s %d\n",
                    //                                                            "last started", Time_string(s.program.started, t),
                    //                                                            "last exit value", s.program.exitStatus);
                    //                                } else
                    //                                        StringBuffer_append(res.outputbuffer,
                    //                                                            "  %-33s\n",
                    //                                                            "not yet started");
                    //                        }
                }
                res.AppendFormat("  {0} {1}\n\n", txtFormatting("data collected", 33), Timing.GetTimestamp(s.collected));
            }
        }

        private string getMonitoringStatus(Service_T s)
        {
            if (s.monitor == MonitMonitorStateType.Monitor_Not)
                return "Not monitored";
            if (s.monitor == MonitMonitorStateType.Monitor_Waiting)
                return "Waiting";
            if (s.monitor == MonitMonitorStateType.Monitor_Init)
                return "Initializing";
            if (s.monitor == MonitMonitorStateType.Monitor_Yes)
                return "Monitored";
            return "";
        }


        private string getServiceStatus(Service_T s)
        {
            var result = "";
            var et = MonitEventTable.Event_Table;

            if (s.monitor == MonitMonitorStateType.Monitor_Not ||
                ((int) s.monitor & (int) MonitMonitorStateType.Monitor_Init) != 0)
            {
                result = getMonitoringStatus(s);
            }
            else if (s.error == 0)
            {
                result = MonitWindowsAgent.statusnames[(int) s.type];
            }
            else
            {
                // In the case that the service has actualy some failure, error will be non zero. We will check the bitmap and print the description of the first error found
                foreach (var ete in et)
                {
                    if ((s.error & (int) ete.id) != 0)
                    {
                        result = (s.error_hint & (int) ete.id) != 0 ? ete.description_changed : ete.description_failed;
                        break;
                    }
                }
            }
            if (s.doaction != MonitActionType.Action_Ignored)
                result += string.Format(" - {0} pending", MonitWindowsAgent.actionnames[(int) s.doaction]);

            return result;
        }

        #endregion
    }
}