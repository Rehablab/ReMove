---
title: 'ReMove–a application to control the commercial robots for mechanic research'
tags:
    - Unity
    - CSharp
    - Rehabilitation
    - Robot control
    - biomechanics
authors:
    - name: Qiang Xu
      affiliation: 1
      orcid: 0000-0001-8781-301X
    - name: Zhi Chen
      affiliation: 2
      orcid: 0000-0002-0202-006X
    - name: Minos.Niu
      orcid:

affiliations:
    - name: Fourier Intelligence inc., Shanghai, China
      index: 1
    - name: Department of Rehabilitation Medicine, Ruijin Hospital, School of Medicine, Shanghai Jiao Tong University, Shanghai, China.
      index: 2
    - name: Laboratory of Neurorehabilitation Engineering, School of Biomedical Engineer-ing, Shang-hai Jiao Tong University, Shanghai, China
      index: 3

date: 12 December 2021
bibliography: paper.bib
---

# Summary

Resistance training has shown efficacy in post-stroke rehabilitation(Flansbjer, Lexell, and Brogårdh 2012)(Ouellette et al. 2004). Further studies should be focused to evaluate the effect of single variables of resistance training, such as intensity, rest interval, and frequency(Gambassi, Coelho-Junior, and Schwingel, n.d.). Thus, having software to create the expected dynamic environment is essential for further studies in biomechanics, motor control, and clinical research. Here we introduce ['ReMove'](https://github.com/Rehablab/ReMove), a programmable application in order to alter movement trajectory and resistance on the commercial upper limb rehabilitation robot. 

# Statement of need

Previous studies confirm that resistance alters the motor control strategies of humans. Add-ing load would induce several physiological changes, including a higher magnitude of mus-cle activities, longer activation period of muscle activation, and different activation patterns of the brain cortex(Gottlieb 1996)(Dai et al. 2001)(Dettmers et al. 1995). These changes may explain the therapeutic effect of resistance training. To understand the control patterns under different resistance, researchers need methods to control the applied load during movement.
In biomechanics, researchers struggle to create resistance of different properties in a real-life scenario, such as adding weight to the barbells or using elastic strings. Once the experi-mental scene is set up, the applied load is constant. Thus, this method cannot implement trial-by-trial modification. What’s more, some properties of resistance are hard to be creat-ed, such as the viscous load or the inertial load in the horizontal plane. 

Some existing software may be used to solve some of these problems, such as LabVIEW(Bishop 2000), H3D, and PsychoPy(Peirce 2007). Based on the functions, the software above can be divided into two categories: robot driving and study arrangement. LabVIEW, a graphical dataflow programming environment for the integration of hardware, is typical of the robot driving software. It has strength in driving robots but weakness in study design. What’s more, LabVIEW shows more applicability in engineering, rather than scien-tific research. PsychoPy, a psychophysics software developed in python for study arrange-ment, is typical of the second category. It has advantages in study design, but needs extra programming to drive a robot. Until now, researchers haven’t had software that gears to the scientific purpose yet. 

In stroke rehabilitation, clinical researchers pay growing attention to the upper-limb function recovery, because the motor control of upper limbs is more complex, responsible for more functions in the activities of daily living, and are difficult to recover. Thus, we introduce ReMove, the programmable application developed by unity to create the different resistance and collect kinematic data on the commercial upper limb rehabilitation robot. Compared to the software above, ReMove is designed only for scientific research, which is capable of driving robots, experimental design, and collecting data. It has applications in mechanic re-search, clinical practice, and clinical study. 

In the field of the motor control study, ReMove provides a way for direct control of the ro-bot to create different dynamic environment, including the magnitude of resistance, the property of resistance, and the movement trajectories. ReMove not only simulate both the inertial and viscous load but also implement the continuous control of applied load just by typing the keyboard. Using ReMove, researchers are freed from the heavy experiment prep-arations (such as lifting 30Kg weight plates) and create the expecting environment in minutes. In the field of clinical practice, ReMove simplifies the customization of movement trajectory and resistance. It enables the therapists who don’t have any programming capabil-ity to set dynamic parameters using a user interface, which is important in the concept of ‘precise rehabilitation’. In the field of clinical study, ReMove is developed based on the commercial upper limb rehabilitation robot – M2pro (Fourier Intelligence inc., Shanghai, China). Based on the initial code, ReMove can get access to most  commercial robots on the market. This may narrow the instrumental error in the multicenter-clinical trials and the bias in the future META analysis.

# Software overview

The overall structure chart is shown in [Figure 1](#refer-anchor-1).

<div id="refer-anchor-1" align=center><img src="./StructureDiagram.jpg" /></div>

<p align="center">Figure 1. The architecture chart of ReMove.</p>

ReMove has three different functional modules, including the customization of movement, device status tracking, and the extraction of kinematic data. 

The main UI of ReMove is shown in [Figure 2](#refer-anchor-2).

<div id="refer-anchor-2" align=center><img src="./MainUI.jpg" /></div>

<p align="center">Figure 2. The main UI of ReMove.</p>

ReMove customizes movement tracks through the grip of M2pro based on the default trial-based paradigm, which is a point-to-point movement. Researchers can freely set the location of start-point and end-point by the user interface within the range of the operation platform (52cm * 36cm). what’s more, as shown in Figure 2.B, ReMove also provides a visual feed-back during the movement trials, displaying the real-time position of the grip and the targets. 

ReMove can extract the kinematic data of the grip of M2pro in 25Hz maximum, including the location and force in two-dimensional coordinate. Meanwhile, ReMove also outputs the corresponding state of the finite state machine in each observation point. Based on that, re-searchers can split the whole kinematic data into every point-to-point movement, which is really convenient for the following analysis including motor learning, muscle synergy calcu-lation, and collaborative analysis. All data extracted is saved in .csv format. 

# Experimental example

To demonstrate the function of ReMove, we collected one set of kinematic data in Ruijin Hospital.

The subject was asked to move towards from start-point to seven different targets set by the therapist who does not have any knowledge of programming. The trajectories and velocity profiles are shown in [Figure 3](#refer-anchor-3).

<div id="refer-anchor-3" align=center><img src="./DataSample.png" /></div>

<p align="center">Figure 3. The data sample of ReMove.</p>

# Acknowledgements

We want to thank Fourier Intelligence inc., for providing M2pro to develop ReMove. We also want to thank Yongjun Qiao in Ruijin hospital for helping us test ReMove.

This word is funded by the General Project of National Natural Science Foundation of China (81971722); Shanghai Science and Technology Commission Project (CZ-201912940); Shanghai Municipal Health Commission Project (2019SY004).

# References

Bishop, Robert H. 2000. _Learning with LabVIEW_. 3. print., Student ed. Menlo Park, Calif.: Addison-Wesley Longman.

Dai, Te, Jing Liu, Vinod Sahgal, Robert Brown, and Guang Yue. 2001. &quot;Relationship between Muscle Output and Functional MRI-Measured Brain Activation.&quot; _Experimental Brain Research_ 140 (3): 290–300. <https://doi.org/10.1007/s002210100815>.

Dettmers, C., G. R. Fink, R. N. Lemon, K. M. Stephan, R. E. Passingham, D. Silbersweig, A. Holmes, M. C. Ridding, D. J. Brooks, and R. S. Frackowiak. 1995. &quot;Relation between Cerebral Activity and Force in the Motor Areas of the Human Brain.&quot; _Journal of Neurophysiology_ 74 (2): 802–15. <https://doi.org/10.1152/jn.1995.74.2.802>.

Flansbjer, U, J Lexell, and C Brogårdh. 2012. &quot;Long-Term Benefits of Progressive Resistance Training in Chronic Stroke: A 4-Year Follow-Up.&quot; _Journal of Rehabilitation Medicine_ 44 (3): 218–21. <https://doi.org/10.2340/16501977-0936>.

Gambassi, Bruno Bavaresco, Hélio José Coelho-Junior, and Paulo Adriano Schwingel. n.d. &quot;Resistance Training and Stroke: A Critical Analysis of Different Training Programs.&quot; _Stroke Research and Treatment_, 12.

Gottlieb, G. L. 1996. &quot;On the Voluntary Movement of Compliant (Inertial-Viscoelastic) Loads by Parcellated Control Mechanisms.&quot; _Journal of Neurophysiology_ 76 (5): 3207–29. <https://doi.org/10.1152/jn.1996.76.5.3207>.

Ouellette, Michelle M., Nathan K. LeBrasseur, Jonathan F. Bean, Edward Phillips, Joel Stein, Walter R. Frontera, and Roger A. Fielding. 2004. &quot;High-Intensity Resistance Training Improves Muscle Strength, Self-Reported Function, and Disability in Long-Term Stroke Survivors.&quot; _Stroke_ 35 (6): 1404–9. <https://doi.org/10.1161/01.STR.0000127785.73065.34>.

Peirce, Jonathan W. 2007. &quot;PsychoPy—Psychophysics Software in Python.&quot; _Journal of Neuroscience Methods_ 162 (1–2): 8–13. <https://doi.org/10.1016/j.jneumeth.2006.11.017>.




