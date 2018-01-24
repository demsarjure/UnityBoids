# clear workspace
rm(list=ls())

# load libraries
library(ggplot2)
library(ggthemes)
library(plyr)
library(cowplot)

# working directory
setwd("D:/Projects/Boids/Assets/R")

# data 5
dfA5 <- data.frame(dfA5 <- read.table("5/A.csv",sep=",",header = TRUE))
dfA5 <- cbind(dfA5, type="Naivno iskanje")
dfKD5 <- data.frame(dfKD5 <- read.table("5/KD.csv",sep=",",header = TRUE))
dfKD5 <- cbind(dfKD5, type="KD Drevo")
dfSP5 <- data.frame(dfSP5 <- read.table("5/SP.csv",sep=",",header = TRUE))
dfSP5 <- cbind(dfSP5, type="Razdelitev prostora")

# data 10
dfA10 <- data.frame(dfA10 <- read.table("10/A.csv",sep=",",header = TRUE))
dfA10 <- cbind(dfA10, type="Naivno iskanje")
dfKD10 <- data.frame(dfKD10 <- read.table("10/KD.csv",sep=",",header = TRUE))
dfKD10 <- cbind(dfKD10, type="KD Drevo")
dfSP10 <- data.frame(dfSP10 <- read.table("10/SP.csv",sep=",",header = TRUE))
dfSP10 <- cbind(dfSP10, type="Razdelitev prostora")

# toss first X frames
X = 15;

dfA5 <- dfA5[dfA5$frame > X,]
dfSP5 <- dfSP5[dfSP5$frame > X,]
dfKD5 <- dfKD5[dfKD5$frame > X,]

dfA10 <- dfA10[dfA10$frame > X,]
dfSP10 <- dfSP10[dfSP10$frame > X,]
dfKD10 <- dfKD10[dfKD10$frame > X,]

# merge df
df5 <- rbind(dfA5,dfKD5)
df5 <- rbind(df5,dfSP5)

df10 <- rbind(dfA10,dfKD10)
df10 <- rbind(df10,dfSP10)

# CI5
dfCI5 <- ddply(df5, .(N,type), summarize, mean=mean(fps), l=quantile(fps, 0.025), h=quantile(fps, c(0.975)))

p1 <- ggplot(data=dfCI5,aes(x=N,y=mean,colour=type,fill=type)) +
  geom_smooth(se = TRUE, size = .5, show.legend=FALSE, method = "gam", formula = y ~ s(log(x))) +
  geom_smooth(se = FALSE, size = .5, method = "gam", formula = y ~ s(log(x))) +
  theme_minimal() +
  labs(title = expression(paste(r[c], " = 5 m")), x = "Number of agents", y = "FPS") +
  theme(legend.title=element_blank(), legend.position="bottom") +
  theme(plot.title = element_text(hjust = 0.5)) +
  scale_y_continuous(breaks=c(0, 30, 60, 90, 120)) +
  scale_colour_colorblind() +
  scale_fill_colorblind() +
  coord_cartesian(ylim=c(0, 120))

# CI10
dfCI10 <- ddply(df10, .(N,type), summarize,  mean=mean(fps), l=quantile(fps, 0.025), h=quantile(fps, c(0.975)))

p2 <- ggplot(data=dfCI10,aes(x=N,y=mean,colour=type,fill=type)) +
  geom_smooth(se = TRUE, size = .5, show.legend=FALSE, method = "gam", formula = y ~ s(log(x))) +
  geom_smooth(se = FALSE, size = .5, method = "gam", formula = y ~ s(log(x))) +
  theme_minimal() +
  labs(title = expression(paste(r[c], " = 10 m")), x = "Number of agents", y = "FPS") +
  theme(legend.title=element_blank(), legend.position="bottom") +
  theme(plot.title = element_text(hjust = 0.5)) +
  scale_y_continuous(breaks=c(0, 30, 60, 90, 120)) +
  scale_colour_colorblind() +
  scale_fill_colorblind() +
  coord_cartesian(ylim=c(0, 120))

p_grid <- plot_grid(p1 + theme(legend.position="none"), p2 + theme(legend.position="none"),
          ncol = 1, nrow = 2, scale = 0.9)

legend <- get_legend(p1)
p <- plot_grid(p_grid, legend, ncol = 1, rel_heights = c(1, .1))

ggsave("figResults.pdf", p, width = 16, height = 12, units = "cm")
