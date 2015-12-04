namespace ChMonitoring
{
    /// <summary>
    /// TODO
    /// </summary>
    class Process
    {
//        /* ------------------------------------------------------------------ Public */


///**
// * Initialize the proc information code
// * @return true if succeeded otherwise false.
// */
//boolean_t init_process_info(void) {
//        memset(&systeminfo, 0, sizeof(SystemInfo_T));
//        gettimeofday(&systeminfo.collected, NULL);
//        if (uname(&systeminfo.uname) < 0) {
//                LogError("'%s' resource monitoring initialization error -- uname failed: %s\n", Run.system->name, STRERROR);
//                return false;
//        }

//        systeminfo.total_cpu_user_percent = -10;
//        systeminfo.total_cpu_syst_percent = -10;
//        systeminfo.total_cpu_wait_percent = -10;

//        return (init_process_info_sysdep());
//}


///**
// * Get the proc infomation (CPU percentage, MEM in MByte and percent,
// * status), enduser version.
// * @param p A Service object
// * @param pid The process id
// * @return true if succeeded otherwise false.
// */
//boolean_t update_process_data(Service_T s, ProcessTree_T *pt, int treesize, pid_t pid) {
//        ASSERT(s);
//        ASSERT(systeminfo.mem_kbyte_max > 0);

//        /* save the previous pid and set actual one */
//        s->inf->priv.process._pid = s->inf->priv.process.pid;
//        s->inf->priv.process.pid  = pid;

//        int leaf;
//        if ((leaf = findprocess(pid, pt, treesize)) != -1) {
//                /* save the previous ppid and set actual one */
//                s->inf->priv.process._ppid             = s->inf->priv.process.ppid;
//                s->inf->priv.process.ppid              = pt[leaf].ppid;
//                s->inf->priv.process.uid               = pt[leaf].uid;
//                s->inf->priv.process.euid              = pt[leaf].euid;
//                s->inf->priv.process.gid               = pt[leaf].gid;
//                s->inf->priv.process.uptime            = Time_now() - pt[leaf].starttime;
//                s->inf->priv.process.children          = pt[leaf].children_sum;
//                s->inf->priv.process.mem_kbyte         = pt[leaf].mem_kbyte;
//                s->inf->priv.process.zombie            = pt[leaf].zombie;
//                s->inf->priv.process.total_mem_kbyte   = pt[leaf].mem_kbyte_sum;
//                s->inf->priv.process.cpu_percent       = pt[leaf].cpu_percent;
//                s->inf->priv.process.total_cpu_percent = pt[leaf].cpu_percent_sum;
//                if (systeminfo.mem_kbyte_max == 0) {
//                        s->inf->priv.process.total_mem_percent = 0;
//                        s->inf->priv.process.mem_percent       = 0;
//                } else {
//                        s->inf->priv.process.total_mem_percent = (int)((double)pt[leaf].mem_kbyte_sum * 1000.0 / systeminfo.mem_kbyte_max);
//                        s->inf->priv.process.mem_percent       = (int)((double)pt[leaf].mem_kbyte * 1000.0 / systeminfo.mem_kbyte_max);
//                }
//        } else {
//                s->inf->priv.process.ppid              = -1;
//                s->inf->priv.process.uid               = -1;
//                s->inf->priv.process.euid              = -1;
//                s->inf->priv.process.gid               = -1;
//                s->inf->priv.process.uptime            = 0;
//                s->inf->priv.process.children          = 0;
//                s->inf->priv.process.total_mem_kbyte   = 0;
//                s->inf->priv.process.total_mem_percent = 0;
//                s->inf->priv.process.mem_kbyte         = 0;
//                s->inf->priv.process.mem_percent       = 0;
//                s->inf->priv.process.cpu_percent       = 0;
//                s->inf->priv.process.total_cpu_percent = 0;
//        }
//        return true;
//}


///**
// * Updates the system wide statistic
// * @return true if successful, otherwise false
// */
//boolean_t update_system_load() {
//        if (Run.doprocess) {
//                ASSERT(systeminfo.mem_kbyte_max > 0);

//                /** Get load average triplet */
//                if (getloadavg_sysdep(systeminfo.loadavg, 3) == -1) {
//                        LogError("'%s' statistic error -- load average gathering failed\n", Run.system->name);
//                        goto error1;
//                }

//                /** Get memory usage statistic */
//                if (! used_system_memory_sysdep(&systeminfo)) {
//                        LogError("'%s' statistic error -- memory usage gathering failed\n", Run.system->name);
//                        goto error2;
//                }
//                systeminfo.total_mem_percent  = (int)(1000 * (double)systeminfo.total_mem_kbyte / (double)systeminfo.mem_kbyte_max);
//                systeminfo.total_swap_percent = systeminfo.swap_kbyte_max ? (int)(1000 * (double)systeminfo.total_swap_kbyte / (double)systeminfo.swap_kbyte_max) : 0;

//                /** Get CPU usage statistic */
//                if (! used_system_cpu_sysdep(&systeminfo)) {
//                        LogError("'%s' statistic error -- cpu usage gathering failed\n", Run.system->name);
//                        goto error3;
//                }

//                return true;
//        }

//error1:
//        systeminfo.loadavg[0] = 0;
//        systeminfo.loadavg[1] = 0;
//        systeminfo.loadavg[2] = 0;
//error2:
//        systeminfo.total_mem_kbyte   = 0;
//        systeminfo.total_mem_percent = 0;
//error3:
//        systeminfo.total_cpu_user_percent = 0;
//        systeminfo.total_cpu_syst_percent = 0;
//        systeminfo.total_cpu_wait_percent = 0;

//        return false;
//}


///**
// * Initialize the process tree
// * @return treesize >= 0 if succeeded otherwise < 0
// */
//int initprocesstree(ProcessTree_T **pt_r, int *size_r, ProcessTree_T **oldpt_r, int *oldsize_r) {
//        ASSERT(pt_r);
//        ASSERT(size_r);
//        ASSERT(oldpt_r);
//        ASSERT(oldsize_r);

//        if (*pt_r) {
//                if (*oldpt_r)
//                        delprocesstree(oldpt_r, oldsize_r);
//                *oldpt_r = *pt_r;
//                *oldsize_r = *size_r;
//                *pt_r = NULL;
//                *size_r = 0;
//        }

//        if ((*size_r = initprocesstree_sysdep(pt_r)) <= 0 || ! *pt_r) {
//                DEBUG("System statistic error -- cannot initialize the process tree -- process resource monitoring disabled\n");
//                Run.doprocess = false;
//                if (*oldpt_r)
//                        delprocesstree(oldpt_r, oldsize_r);
//                return -1;
//        } else if (! Run.doprocess) {
//                DEBUG("System statistic -- initialization of the process tree succeeded -- process resource monitoring enabled\n");
//                Run.doprocess = true;
//        }

//        int oldentry;
//        ProcessTree_T *pt = *pt_r;
//        ProcessTree_T *oldpt = *oldpt_r;
//        for (int i = 0; i < (volatile int)*size_r; i ++) {
//                if (oldpt && ((oldentry = findprocess(pt[i].pid, oldpt, *oldsize_r)) != -1)) {
//                        pt[i].cputime_prev = oldpt[oldentry].cputime;
//                        pt[i].time_prev    = oldpt[oldentry].time;

//                        /* The cpu_percent may be set already (for example by HPUX module) */
//                        if (pt[i].cpu_percent  == 0 && pt[i].cputime_prev != 0 && pt[i].cputime != 0 && pt[i].cputime > pt[i].cputime_prev) {
//                                pt[i].cpu_percent = (int)((1000 * (double)(pt[i].cputime - pt[i].cputime_prev) / (pt[i].time - pt[i].time_prev)) / systeminfo.cpus);
//                                if (pt[i].cpu_percent > 1000)
//                                        pt[i].cpu_percent = 1000;
//                        }
//                } else {
//                        pt[i].cputime_prev = 0;
//                        pt[i].time_prev    = 0.0;
//                        pt[i].cpu_percent  = 0;
//                }

//                if (pt[i].pid == pt[i].ppid) {
//                        pt[i].parent = i;
//                        continue;
//                }

//                if ((pt[i].parent = findprocess(pt[i].ppid, pt, *size_r)) == -1) {
//                        /* Parent process wasn't found - on Linux this is normal: main process with PID 0 is not listed, similarly in FreeBSD jail.
//                         * We create virtual process entry for missing parent so we can have full tree-like structure with root. */
//                        int j = (*size_r)++;

//                        pt = RESIZE(*pt_r, *size_r * sizeof(ProcessTree_T));
//                        memset(&pt[j], 0, sizeof(ProcessTree_T));
//                        pt[j].ppid = pt[j].pid  = pt[i].ppid;
//                        pt[i].parent = j;
//                }

//                if (! connectchild(pt, pt[i].parent, i)) {
//                        /* connection to parent process has failed, this is usually caused in the part above */
//                        DEBUG("System statistic error -- cannot connect process id %d to its parent %d\n", pt[i].pid, pt[i].ppid);
//                        pt[i].pid = 0;
//                        continue;
//                }
//        }

//        /* The main process in Solaris zones and FreeBSD host doesn't have pid 1, so try to find process which is parent of itself */
//        int root = -1;
//        for (int i = 0; i < *size_r; i++) {
//                if (pt[i].pid == pt[i].ppid) {
//                        root = i;
//                        break;
//                }
//        }

//        if (root == -1) {
//                DEBUG("System statistic error -- cannot find root process id\n");
//                if (*oldpt_r)
//                        delprocesstree(oldpt_r, oldsize_r);
//                if (*pt_r)
//                        delprocesstree(pt_r, size_r);
//                return -1;
//        }

//        fillprocesstree(pt, root);

//        return *size_r;
//}


///**
// * Search a leaf in the processtree
// * @param pid  pid of the process
// * @param pt  processtree
// * @param treesize  size of the processtree
// * @return process index if succeeded otherwise -1
// */
//int findprocess(int pid, ProcessTree_T *pt, int treesize) {
//        ASSERT(pt);

//        if (treesize <= 0)
//                return -1;

//        for (int i = 0; i < treesize; i++)
//                if (pid == pt[i].pid)
//                        return i;

//        return -1;
//}


//time_t getProcessUptime(pid_t pid, ProcessTree_T *pt, int treesize) {
//        if (pt) {
//                int leaf = findprocess(pid, pt, treesize);
//                return (time_t)((leaf >= 0 && leaf < treesize) ? Time_now() - pt[leaf].starttime : -1);
//        } else {
//                return 0;
//        }
//}


///**
// * Delete the process tree
// */
//void delprocesstree(ProcessTree_T **reference, int *size) {
//        ProcessTree_T *pt = *reference;
//        if (pt) {
//                for (int i = 0; i < *size; i++) {
//                        FREE(pt[i].cmdline);
//                        FREE(pt[i].children);
//                }
//                FREE(pt);
//                *reference = NULL;
//                *size = 0;
//        }
//}


//void process_testmatch(char *pattern) {
//#ifdef HAVE_REGEX_H
//        regex_t *regex_comp;
//        int reg_return;

//        NEW(regex_comp);
//        if ((reg_return = regcomp(regex_comp, pattern, REG_NOSUB|REG_EXTENDED))) {
//                char errbuf[STRLEN];
//                regerror(reg_return, regex_comp, errbuf, STRLEN);
//                regfree(regex_comp);
//                FREE(regex_comp);
//                printf("Regex %s parsing error: %s\n", pattern, errbuf);
//                exit(1);
//        }
//#endif
//        initprocesstree(&ptree, &ptreesize, &oldptree, &oldptreesize);
//        if (Run.doprocess) {
//                int count = 0;
//                printf("List of processes matching pattern \"%s\":\n", pattern);
//                printf("------------------------------------------\n");
//                for (int i = 0; i < ptreesize; i++) {
//                        boolean_t match = false;
//                        if (ptree[i].cmdline && ! strstr(ptree[i].cmdline, "procmatch")) {
//#ifdef HAVE_REGEX_H
//                                match = regexec(regex_comp, ptree[i].cmdline, 0, NULL, 0) ? false : true;
//#else
//                                match = strstr(ptree[i].cmdline, pattern) ? true : false;
//#endif
//                                if (match) {
//                                        printf("\t%s\n", ptree[i].cmdline);
//                                        count++;
//                                }
//                        }
//                }
//                printf("------------------------------------------\n");
//                printf("Total matches: %d\n", count);
//                if (count > 1)
//                        printf("WARNING: multiple processes matched the pattern. The check is FIRST-MATCH based, please refine the pattern\n");
//        }
    }
}